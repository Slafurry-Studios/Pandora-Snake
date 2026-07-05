public static class BuildingUtils
{
    public static bool CanFitBuildingWithMargins(int[,] grid, int startX, int startY, int bWidth, int bHeight, int mLeft, int mRight, int mBottom, int mTop)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // 1. Is the physical building itself out of bounds?
        if (startX < 0 || startY < 0 || startX + bWidth > width || startY + bHeight > height)
            return false;

        // 2. Calculate the required empty footprint (Building Size + Margins)
        int checkX = startX - mLeft;
        int checkY = startY - mBottom;
        int checkWidth = bWidth + mLeft + mRight;
        int checkHeight = bHeight + mBottom + mTop;

        // 3. Scan the entire footprint
        for (int x = checkX; x < checkX + checkWidth; x++)
        {
            for (int y = checkY; y < checkY + checkHeight; y++)
            {
                if (x < 0 || y < 0 || x >= width || y >= height)
                    return false;

                if (grid[x, y] != 0)
                    return false;
            }
        }
        return true;
    }

    public static void MarkGridOccupied(int[,] grid, int startX, int startY, int bWidth, int bHeight)
    {
        for (int x = startX; x < startX + bWidth; x++)
        {
            for (int y = startY; y < startY + bHeight; y++)
            {
                grid[x, y] = 2; // 2 = building
            }
        }
    }

    public static void MarkGridReserved(int[,] grid, int startX, int startY, int rWidth, int rHeight)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int x0 = UnityEngine.Mathf.Max(0, startX);
        int y0 = UnityEngine.Mathf.Max(0, startY);
        int x1 = UnityEngine.Mathf.Min(width, startX + rWidth);
        int y1 = UnityEngine.Mathf.Min(height, startY + rHeight);

        for (int x = x0; x < x1; x++)
        {
            for (int y = y0; y < y1; y++)
            {
                if (grid[x, y] == 0) grid[x, y] = 3; // 3 = reserved plaza
            }
        }
    }
}