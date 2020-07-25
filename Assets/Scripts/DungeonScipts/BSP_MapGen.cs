using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

// Requirements of a map generator
// provide the int[,] tileMap
// provide (limeted) write access to tile map (amend tile value)
// provide read access to room info
// provide read access to Cache info

public struct Segment
{
    // Pos = centre
    public int xPos;
    public int yPos;
    public int width;
    public int height;

    public string segmentKey;
    public Room segmentRoom;
}

public struct Room
{
    public int roomWidth;
    public int roomHeight;
    public int roomLeft;
    public int roomBottom;
    public RoomCache roomCache;

    public List<Vector2Int> roomDoors;
}

public struct RoomCache
{
    public Vector2Int position;
    // List of guardians
    public List<MonsterInfo> guardians;
    // list of loot
    public int goldLoot;
    // other info etc
}

public class BSP_MapGen
{
    // Map Gen Variables
    List<List<List<Segment>>> BSPMap = new List<List<List<Segment>>>();
    readonly public int MAP_WIDTH;
    readonly public int MAP_HEIGHT;
    readonly int floorDifficulty;
    const float MAX_DIVIDE_RATIO = 0.70f;
    const float MIN_DIVIDE_RATION = 0.30f;
    const int DIVIDE_COUNT = 4;

    int[,] map;
    public int[,] GetMap { get { return map; } }
    public void AmendMap(Vector2Int pos, int value) { map[pos.x, pos.y] = value; }
    Vector2Int mapUpStairs;
    public Vector2Int GetMapUpStairs { get { return mapUpStairs; } }
    Vector2Int mapDownStairs;
    public Vector2Int GetMapDownStairs { get { return mapDownStairs; } }

    List<RoomCache> cacheList = new List<RoomCache>();
    public List<RoomCache> GetCacheList { get { return cacheList; } }

    // Room Gen variables
    const int ROOM_MIN_WIDTH = 2;
    const int ROOM_MIN_HEIGHT = 2;

    public RoomCache FindCache(Vector2Int pos)
    {
        foreach (var segment in BSPMap[0][0])
        {
            if (segment.segmentRoom.roomCache.position == pos)
                return segment.segmentRoom.roomCache;
        }

        Debug.LogError("No cache has been found at the pos: X-" + pos.x + " Y-" + pos.y);
        return new RoomCache();
    }

    public BSP_MapGen(int map_Width, int map_Height, int difficulty)
    {
        MAP_WIDTH = map_Width;
        MAP_HEIGHT = map_Height;
        floorDifficulty = difficulty;
    }

    public void LoadBSPDungeon(int[,] newMap, List<RoomCache> newCacheList, Vector2Int upStairPos, Vector2Int downStairPos)
    {
        map = newMap;
        cacheList = newCacheList;
        // upstairs
        mapUpStairs = upStairPos;
        // down stairs
        mapDownStairs = downStairPos;
    }

    public void GenerateBSPDungeon()
    {
        // Reset Map
        map = new int[(int)MAP_WIDTH, (int)MAP_HEIGHT];
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                map[x, y] = 0;
            }
        }

        BSPStage();
        RoomPlacementStage();
        FirstPassCorridorsStage();
        SecondPassCorridorsStage();

        // Find Entrance (Up Stairs) to Level
        mapUpStairs = FindUpStairsPosition();
        mapDownStairs = FindDownStairsPosition();
    }

    Vector2Int FindUpStairsPosition()
    {
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                if (map[x, y] == 3)
                    return new Vector2Int(x, y);
            }
        }

        Debug.Log("No UpStairs tile found!");
        return new Vector2Int(0, 0);
    }
    Vector2Int FindDownStairsPosition()
    {
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                if (map[x, y] == 4)
                    return new Vector2Int(x, y);
            }
        }

        Debug.Log("No DownStairs tile found!");
        return new Vector2Int(0, 0);
    }


    // BSP - partition the space up into randomly sized areas
    #region
    void BSPStage()
    {
        // Map
        //      ________________
        //      |              |
        //      |              |
        //      |              |
        //      |              |
        //      |______________|
        //
        //      _________________
        //      |A      |B      |
        //      |       |       |
        //      |       |       |
        //      |       |       |
        //      |_______|_______|
        //  
        //      _________________
        //      |A1     |B1     |
        //      |       |       |
        //      |_______|_______|
        //      |A2     |B2     |
        //      |_______|_______|
        //
        //  List<Segment> - contains all the segments in the map
        //  List<Segment> A, List<Segment> B - contains all the segments split into 2 lists By the first divide, as shown above
        //  List<Segment> A1, List<Segment> A2, List<Segment> B1, List<Segment> B2 - contains all the segments split into 4 lists By the first 2 divides, as shown above
        //  ... and so on....

        BSPMap = new List<List<List<Segment>>>();
                        List<List<Segment>> startLevel = new List<List<Segment>>();
                            List<Segment> root = new List<Segment>
        {
            new Segment
            {
                xPos = 0,
                yPos = 0,
                width = MAP_WIDTH,
                height = MAP_HEIGHT,

                segmentKey = "BASE"
            }
        };
        startLevel.Add(root);
        BSPMap.Add(startLevel);

        #region Step 1 - Build List structure
        for (int i = 0; i < DIVIDE_COUNT; i++)
        {
            // Take last element of List
            var lastMapLevel = BSPMap[BSPMap.Count - 1];

            List<List<Segment>> newLevel = new List<List<Segment>>();
            // Decide if cut virtically or horizontaly
            if (i % 2 == 1)
            {
                foreach (var segmentList in lastMapLevel)
                {
                    foreach (var segment in segmentList)
                    {
                        newLevel.AddRange(SplitSegmentHorizontal(segment));
                    }

                }
            }
            else
            {
                foreach (var segmentList in lastMapLevel)
                {
                    foreach (var segment in segmentList)
                    {
                        newLevel.AddRange(SplitSegmentVertical(segment));
                    }

                }
            }

            // Add New Level to the Map
            BSPMap.Add(newLevel);
        }
        #endregion

        #region Step 2 - Replace individual large segments with list of all small segments that space contains
        foreach (var level in BSPMap)
        {
            foreach (var segmentList in level)
            {
                var temp = FillSegmentList(segmentList[0]);
                segmentList.RemoveAt(0);
                segmentList.AddRange(temp);
            }
        }
        #endregion
    }

    private List<List<Segment>> SplitSegmentHorizontal(Segment input)
    {
        List<List<Segment>> output = new List<List<Segment>>();

        float seg1Ratio = RandomRatio();

        int min = 4; // min room size (2) + wall on either side (1+1)
        int i_Seg1Height = Mathf.Clamp((int)(input.height * seg1Ratio), min, input.height - min);
        int i_Seg2Height = (int)input.height - i_Seg1Height;

        Segment segment1 = new Segment()
        {
            // Bottom left of top segment = 
            // xPos stays the same
            // ypos = +height of BOTTOM segement
            xPos = input.xPos,
            yPos = input.yPos + i_Seg2Height,
            width = input.width,
            height = i_Seg1Height,

        };
        output.Add(new List<Segment>() { segment1 });
        Segment segment2 = new Segment()
        {
            // Bottom Left of Bottom segment = 
            // xPos stays the same
            // yPos stays the same
            xPos = input.xPos,
            yPos = input.yPos,
            width = input.width,
            height = i_Seg2Height,
        };
        output.Add(new List<Segment>() { segment2 });

        return output;
    }

    private List<List<Segment>> SplitSegmentVertical(Segment input)
    {
        List<List<Segment>> output = new List<List<Segment>>();

        float seg1Ratio = RandomRatio();

        int min = 4; // min room size (2) + wall on either side (1+1)
        int i_Seg1Width = Mathf.Clamp((int)(input.width * seg1Ratio), min, input.width - 4);
        int i_Seg2Width = (int)input.width - i_Seg1Width;

        Segment segment1 = new Segment()
        {
            // Bottom left of new Left Segment = 
            // xPos stays the same
            // yPos stays the same
            xPos = input.xPos,
            yPos = input.yPos,
            width = i_Seg1Width,
            height = input.height
        };
        output.Add(new List<Segment>() { segment1 });
        Segment segment2 = new Segment()
        {
            // Bottom left of new Right Segment = 
            // xPos = + width of left segment
            // yPos stays the same
            xPos = input.xPos + i_Seg1Width,
            yPos = input.yPos,
            width = i_Seg2Width,
            height = input.height
        };
        output.Add(new List<Segment>() { segment2 });

        return output;
    }

    private float RandomRatio()
    {
        return UnityEngine.Random.Range(MIN_DIVIDE_RATION, MAX_DIVIDE_RATIO);
    }

    private List<Segment> FillSegmentList(Segment region)
    {
        List<Segment> output = new List<Segment>();
        int regionLeft = region.xPos;
        int regionRight = region.xPos + region.width;
        int regionTop = region.yPos + region.height;
        int regionBottom = region.yPos;

        // For each segment created in the map
        foreach (var segmentList in BSPMap[BSPMap.Count - 1])
        {
            foreach (var segment in segmentList)
            {
                float centerXPos = segment.xPos + segment.width / 2;
                float centerYPos = segment.yPos + segment.height / 2;
                // if the centre pos, is inside the given region
                // Add to the List
                if (centerXPos > regionLeft &&
                    centerXPos < regionRight &&
                    centerYPos < regionTop &&
                    centerYPos > regionBottom)
                {
                    output.Add(segment);
                }
            }
        }
        return output;
    }
    #endregion

    // Room placement - Asign a randomly sized room to each of the BSP segments
    // The room can not be larger than the segemnt, and is positioned randomly within the segment, 
    // it can not be placed so that it 'spills' out of the segment
    #region
    void RoomPlacementStage()
    {
        // Update the Map with the new rooms
        for (int i = 0; i < BSPMap[0][0].Count; i++)
        {
            // have to 'replace' segment in question with a copy including the room information, 
            // unsure why I was unable to simply amend the 'Room' property
            BSPMap[0][0][i] = BuildRoomInSegment(BSPMap[0][0][i]);
        }
    }

    private Segment BuildRoomInSegment(Segment segment)
    {
        int left = (int)segment.xPos;
        int bottom = (int)segment.yPos;

        // max size of the room is width-1 x height-1
        // This is because there must be enouigh space to have the wall all around the edge

        // Min size of the room can be fixed to an arbituary 2x2, 
        // Just to prevent tiny pointless rooms

        // Get random room size
        Room room = new Room();
        room.roomWidth = UnityEngine.Random.Range(ROOM_MIN_WIDTH, segment.width - 1);
        room.roomHeight = UnityEngine.Random.Range(ROOM_MIN_HEIGHT, segment.height - 1);
        // Get random room start pos - 
        room.roomLeft = UnityEngine.Random.Range(left + 1, left + segment.width - (room.roomWidth + 1));
        room.roomBottom = UnityEngine.Random.Range(bottom + 1, bottom + segment.height - (room.roomHeight + 1));
        // Place cache in room
        room.roomCache.position = GetRandomPointInRoom(room);
        // Set Cache Guardian + Loot
        // Dungeon floor difficulty level.
        // Cache difficulty level = floor difficulty +- 1 (min cap set to 1)
        // Each Monster type has a difficulty level.
        // The map gen 'spends' its cache difficulty level to buy monsters to defend it.
        int cacheDifficulty = Mathf.Max(1, floorDifficulty + Random.Range(-1, 2)); // <- NOTE max random range is exclusive
        room.roomCache.guardians = PopulateGuardList(cacheDifficulty);
        // Gold = 10 * Cache difficulty level
        room.roomCache.goldLoot = cacheDifficulty * 10;

        // TODO... look inot this, im not sure i like this. 
        // I am only making the list of caches like this to work with Load and save easier.
        // and i am only doing this the easier way as i 'think' i will not be keeping dungeon gen this way long term
        cacheList.Add(room.roomCache);

        segment.segmentRoom = room;

        // Adjust tile map array to include new room
        for (int x = room.roomLeft; x < room.roomLeft + room.roomWidth; x++)
        {
            for (int y = room.roomBottom; y < room.roomBottom + room.roomHeight; y++)
            {
                map[x, y] = 1;
            }
        }
        // Adjust tile map array to include cache
        map[room.roomCache.position.x, room.roomCache.position.y] = 2;

        return segment;
    }

    private List<MonsterInfo> PopulateGuardList(int CacheDifficulty)
    {
        List<MonsterInfo> guardList = new List<MonsterInfo>();
        int pointsToSpend = CacheDifficulty;
        List<MonsterInfo> availableCombatants = MonsterDataReader.GetAvailableMonstersForDifficulty(pointsToSpend);
        while (guardList.Count <= 6 && availableCombatants.Count > 0)
        {
            int randIndex = Random.Range(0, availableCombatants.Count);
            guardList.Add(availableCombatants[randIndex]);
            pointsToSpend -= availableCombatants[randIndex].DifficultyLevel;

            availableCombatants = MonsterDataReader.GetAvailableMonstersForDifficulty(pointsToSpend);
        }

        if (guardList.Count < 1)
        {
            Debug.LogError("no guards available for cache with difficulty: " + CacheDifficulty);
        }

        return guardList;
    }

    private Vector2Int GetRandomPointInRoom(Room room)
    {
        Vector2Int output = new Vector2Int();
        output.x = UnityEngine.Random.Range(room.roomLeft, room.roomLeft + room.roomWidth);
        output.y = UnityEngine.Random.Range(room.roomBottom, room.roomBottom + room.roomHeight);

        return output;
    }
    #endregion

    // First pass of corridor connections - Pair up a number of the rooms previously created
    // Use A* pathfinding to create a path between each pair.
    #region
    void FirstPassCorridorsStage()
    {
        // Get List of Random indices for segments to use
        // Pairs up all the rooms (currently)
        // Make sure it is a even number
        List<int> segmentIndices = new List<int>();
        int maxIndex = BSPMap[0][0].Count;
        segmentIndices = GetRandomIndexList(maxIndex, BSPMap[0][0].Count);

        List<int> impassableList = new List<int>();
        Dictionary<int, int> costModList = new Dictionary<int, int>();
        costModList.Add(0, 40);
        costModList.Add(1, 10);
        costModList.Add(2, 10);
        costModList.Add(3, 10);
        costModList.Add(4, 10);
        AstarPathfinder pathfinder = new AstarPathfinder(map, costModList, impassableList);
        // loop through randomly selected segments
        for (int i = 0; i < segmentIndices.Count; i += 2)
        {
            // Start node will be in BSPMap[0][0][i]
            Vector2Int startNode = BSPMap[0][0][segmentIndices[i]].segmentRoom.roomCache.position;
            // Target node will be in BSPMap[0][0][i + 1]
            Vector2Int targetNode = BSPMap[0][0][segmentIndices[i + 1]].segmentRoom.roomCache.position;            

            List<Node> path = pathfinder.StartPathfinder(startNode, targetNode);
            foreach (var node in path)
            {
                // If the path passes through walls
                // change walls to floor tiles
                if (map[node.xPos, node.yPos] == 0)
                    map[node.xPos, node.yPos] = 1;
            }

        }
    }

    private List<int> GetRandomIndexList(int maxIndex, int countRequired)
    {
        List<int> indexList = new List<int>();
        if (countRequired > maxIndex)
        {
            Debug.LogError("GetRandomIndexList has been passed incorrect values. maxIndex: " + maxIndex + " countRequired: " + countRequired);
        }

        List<int> tempList = new List<int>();
        for (int i = 0; i < maxIndex; i++)
        {
            tempList.Add(i);
        }

        for (int i = 0; i < countRequired; i++)
        {
            int index = tempList[UnityEngine.Random.Range(0, tempList.Count)];
            indexList.Add(index);
            tempList.Remove(index);
        }

        return indexList;
    }
    #endregion

    // Second pass of corridor connections - Assign an 'Up Stairs' tile in one of the rooms,
    // Ensure there is a path from this location to every other room
    // Assign of of the distant rooms to contain the 'Down Stairs'.
    #region
    // Up/Down stairs
    Vector2Int UpStairs;
    Vector2Int DownStairs;

    void SecondPassCorridorsStage()
    {
        List<int> impassableList = new List<int>();
        Dictionary<int, int> costModList = new Dictionary<int, int>();
        costModList.Add(0, 1000);
        costModList.Add(1, 10);
        costModList.Add(2, 10);
        costModList.Add(3, 10);
        costModList.Add(4, 10);
        AstarPathfinder pathfinder = new AstarPathfinder(map, costModList, impassableList);

        // Pick random Room
        // Pick Random tile in that room
        UpStairs = BSPMap[0][0][UnityEngine.Random.Range(0, BSPMap[0][0].Count)].segmentRoom.roomCache.position;// GetRandomPointInRoom(BSPMap[0][0][UnityEngine.Random.Range(0, BSPMap[0][0].Count)].segmentRoom);
        // Assign 'Up Stairs' to that tile
        map[UpStairs.x, UpStairs.y] = 3;

        int longestPathLength = 0;
        // loop through all Segments
        for (int i = 0; i < BSPMap[0][0].Count; i++)
        {
            // Ensure there is a path from the 'Up Stairs' to each room
            Vector2Int startNode = UpStairs;
            Vector2Int targetNode = BSPMap[0][0][i].segmentRoom.roomCache.position;// GetRandomPointInRoom(BSPMap[0][0][i].segmentRoom);                

            List<Node> path = pathfinder.StartPathfinder(startNode, targetNode);
            foreach (var node in path)
            {
                // If the path passes through walls
                // change walls to floor tiles
                if (map[node.xPos, node.yPos] == 0)
                    map[node.xPos, node.yPos] = 1;
            }
            if (path.Count > longestPathLength)
            {
                longestPathLength = path.Count;
                // The 'Down Stairs' tile is replaced with the new longestPath target 
                DownStairs = new Vector2Int(targetNode.x, targetNode.y);
            }
        }

        // Use longest path from 2nd pass to place 'Down Stairs'
        map[DownStairs.x, DownStairs.y] = 4;
    }
    #endregion
}
