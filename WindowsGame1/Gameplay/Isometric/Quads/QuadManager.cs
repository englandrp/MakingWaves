using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Diagnostics;

namespace WindowsGame1
{
    class QuadManager
    {
        static Point gridSize = new Point(9, 9); //number of squares/quads
        static float tileSize = 15.0f; //in game width/length of a tile
        static float halfTileSize = tileSize / 2.0f; //half tile size for finding centre
        Point gridOffset = new Point(0, 0); 

        public Point CurrentTilePos;


        QuadTile cursorTile;//pink cross cursor made of 2 quads
        QuadTile cursorTile2;
        public Point cursorPos = new Point(6, 6);

        QuadTile[,] theTiles = new QuadTile[gridSize.X, gridSize.Y]; //list of quads for drawing, corresponds to list of squares
        CompleteSquare[,] _squares = new CompleteSquare[gridSize.X, gridSize.Y]; //list of squares for pathfinding
        List<Vector2> thePath; //the path Boris will follow from Square.Boris to Square.Destination
        Point[] _movements; //the 8 directions Boris can walk in
        bool ignorewalls = false; //not quite implemented, would cause Boris to go straight to a hard coded destination rather than pathfinding
        
        public CompleteSquare[,] Squares
        {
            get { return _squares; }
            set { _squares = value; }
        }

        public QuadManager(Game1 game,Point gridOffset)
        {
          //  this.gridOffset = new Point(-gridOffset.X,-gridOffset.Y);
            for (int i = 0; i < gridSize.X; i++) //populate 2d list of quads for drawing
            {
                for (int j = 0; j < gridSize.Y; j++)
                {
                  QuadTile quadTile = new QuadTile(Vector3.Zero, Vector3.Up, Vector3.Up, tileSize, tileSize, new Vector3(tileSize * (i + this.gridOffset.X), 1.0f, tileSize * (j + this.gridOffset.Y)), game.GraphicsDevice);
                    theTiles[i,j] = quadTile;
                }
            }
            Vector2 cursorLoc = ReturnTilePos(cursorPos); //create cursor for realtime creation of walls etc from quads
            cursorTile = new QuadTile(Vector3.Zero, Vector3.Up, Vector3.Up, tileSize + 10, tileSize - 10, new Vector3(cursorLoc.X, 1.5f, cursorLoc.Y), game.GraphicsDevice);
            cursorTile.theQuad.changeColor(Color.Purple);
            cursorTile.theAlpha = 0.2f;
            cursorTile2 = new QuadTile(Vector3.Zero, Vector3.Up, Vector3.Up, tileSize - 10, tileSize + 10, new Vector3(cursorLoc.X, 1.5f, cursorLoc.Y), game.GraphicsDevice);
            cursorTile2.theQuad.changeColor(Color.Purple);
            cursorTile2.theAlpha = 0.2f;

            _movements = new Point[] //possible directions for Boris to move, up down left right and diagonals
            {
                new Point(0, -1),  new Point(1, 0), new Point(-1, 0), new Point(0, 1),
                new Point(-1, -1), new Point(1, -1),new Point(1, 1),  new Point(-1, 1)
            };

            this.ClearSquares();
            this.ClearLogic();
          
            try
            {
                this.ReadMap();
                this.Pathfind(false);
            }
            catch
            {
             //   toolStripStatusLabel1.Text = "IO Error";
            }
            finally
            {
                this.UpdateTileColors();
            }
        }

        //Reads in the map file from .txt, and populates the squares list
        public void ReadWalls()
        {
            using (StreamReader reader = new StreamReader("Content\\map1.txt"))
            {
                int lineNum = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    char[] parts = line.ToCharArray();
                    for (int i = 0; i < parts.Length && i < gridSize.X; i++)
                    {
                                if(parts[i] != 'H' && parts[i] != 'M')
                                {
                                    Squares[i, lineNum].FromChar(parts[i]);
                                }
                    }
                    lineNum++;
                }
            }
        }

        public void ReadMap()
        {
            using (StreamReader reader = new StreamReader("Content\\map1.txt"))
            {
                int lineNum = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    char[] parts = line.ToCharArray();
                    for (int i = 0; i < parts.Length && i < gridSize.X; i++)
                    {
                            Squares[i, lineNum].FromChar(parts[i]);
                    }
                    lineNum++;
                }
            }
        }


        //replaces the destination tile, sets nearest tile to Boris as the BorisTile (new path start point), updates the path
        public void CreateEndTile(Vector2 borisPos, Point newpoint, bool ignorewalls)
        {
            borisPos = OffsetPosition(borisPos);
            newpoint = OffsetPoint(newpoint);
            //if(ignorewalls == false)
            //{
            Point destinationSquare = FindCode(SquareContent.Destination);
            if (destinationSquare != new Point(-1, -1))
            {
                _squares[destinationSquare.X, destinationSquare.Y].ContentCode = SquareContent.Empty; //delete the last destinationsquare
            }
            _squares[newpoint.X, newpoint.Y].ContentCode = SquareContent.Destination;
         
            Point borisNearestTile = new Point((int)((borisPos.X - 7.5f) / tileSize) + 1, (int)((borisPos.Y - 7.5f) / tileSize) + 1);
           
            Point borisSquare = FindCode(SquareContent.Boris);
            if (borisSquare != new Point(-1, -1))
            {
                _squares[borisSquare.X, borisSquare.Y].ContentCode = SquareContent.Empty; // delete the last borissquare
            }
            _squares[borisNearestTile.X, borisNearestTile.Y].ContentCode = SquareContent.Boris;
            
            this.ClearLogic();
            try
            {
                if (borisNearestTile != newpoint)
                {
       
                    this.Pathfind(ignorewalls);
                }
            }
            catch
            {
                //   toolStripStatusLabel1.Text = "IO Error";
            }
            finally
            {
                this.UpdateTileColors();
            }
        }

        //run path finding search
        public List<Vector2> ReturnPath(bool ignorewalls)
        {
            this.ignorewalls = ignorewalls;
            List<Vector2> thePath = new List<Vector2>();

            Point currentBorisPoint = FindCode(SquareContent.Boris);
            thePath.Add(ReturnTilePos(currentBorisPoint)); //start path with current position
            Point currentDestinationPoint = FindCode(SquareContent.Destination);
            if (currentDestinationPoint.X == -1 && currentDestinationPoint.Y == -1) //check destination exists
            {
                return thePath;
            }

            bool finished = false;
            while (finished != true) 
            {
                foreach (Point movementDirection in _movements) //check in all directions for a possible move
                {
                    Point potentialNextTile = new Point(movementDirection.X + currentBorisPoint.X, movementDirection.Y + currentBorisPoint.Y); 

                    if (ValidCoordinates(potentialNextTile.X, potentialNextTile.Y))
                    {
                        Vector2 nextPos = (ReturnTilePos(potentialNextTile));
                        Vector2 destinationPos = ReturnTilePos(currentDestinationPoint);

                        if (nextPos == destinationPos)
                        {
                            finished = true;
                            if (thePath.Contains(nextPos) == false)
                            {
                                thePath.Add(OffsetPosition( nextPos));
                                currentBorisPoint = potentialNextTile;
                                break;
                            }
                        }

                        if (_squares[potentialNextTile.X, potentialNextTile.Y].IsPath)
                        {
                            if (thePath.Contains(nextPos) == false)
                            {
                                thePath.Add(OffsetPosition( nextPos));
                                currentBorisPoint = potentialNextTile;
                                break;
                            }
                        }
                    }
                }
            }

 
            return thePath;
        }



        //Find path from hero to monster. First, get coordinates of hero.
        public void Pathfind(bool ignorewalls)
        {
            this.ignorewalls = ignorewalls;
            Point startingPoint = FindCode(SquareContent.Boris);
            int heroX = startingPoint.X;
            int heroY = startingPoint.Y;
            if (heroX == -1 || heroY == -1)
            {
                return;
            }

            _squares[heroX, heroY].DistanceSteps = 0; //Boris starts at distance of 0.

            while (true)
            {
                bool madeProgress = false;
                foreach (Point mainPoint in AllSquares()) // Look at each square on the board.
                {
                    int x = mainPoint.X;
                    int y = mainPoint.Y;

                    if (SquareOpen(x, y)) //If the square is open, look through valid moves given the coordinates of that square.
                    {
                        int passHere = _squares[x, y].DistanceSteps;

                        foreach (Point movePoint in ValidMoves(x, y))
                        {
                            int newX = movePoint.X;
                            int newY = movePoint.Y;
                            int newPass = passHere + 1;

                            if (_squares[newX, newY].DistanceSteps > newPass)
                            {
                                _squares[newX, newY].DistanceSteps = newPass;
                                madeProgress = true;
                            }
                        }
                    }
                    else
                    {
                        x = 10;
                    }
            
                }
                if (!madeProgress)
                {
                    break;
                }
            }

            this.HighlightPath();
        }

        // Mark the path from Boris to destination
        public void HighlightPath()
        {
            //if (ignorewalls == true)
            //{
            //    this.ReadWalls();
            //    //   this.UpdateTileColors();
            //}
            Point startingPoint = FindCode(SquareContent.Destination);
            int pointX = startingPoint.X;
            int pointY = startingPoint.Y;
            if (pointX == -1 && pointY == -1)
            {
                return;
            }

            while (true) // Look through each direction and find the square  with the lowest number of steps marked.
            {
                Point lowestPoint = Point.Zero;
                int lowest = 10000;
                List<Point> pospoints = new List<Point>();

                foreach (Point movePoint in ValidMoves(pointX, pointY))
                {
                    int count = _squares[movePoint.X, movePoint.Y].DistanceSteps;
                    if (count < lowest)
                    {
                        pospoints.Clear();
                        lowest = count;
                    }
                    if (count == lowest)
                    {
                        pospoints.Add(new Point(movePoint.X, movePoint.Y));
                    }
                }
                Point chosePoint = new Point();
                int distance = 10000;
                foreach (Point thePoint in pospoints)
                {
                    int intone = thePoint.X - pointX;
                    int inttwo = thePoint.Y - pointY;
                    if (intone < 0)
                        intone = -intone;
                    if (inttwo < 0)
                        inttwo = -inttwo;
                    int posdistance = intone + inttwo;
                    if (posdistance < distance)
                    {
                        distance = posdistance;
                        chosePoint = new Point(thePoint.X, thePoint.Y);
                    }
                }
                lowestPoint.X = chosePoint.X;
                lowestPoint.Y = chosePoint.Y;
                distance = 10000;
                pospoints.Clear();
                if (lowest != 10000)
                {
                    _squares[lowestPoint.X, lowestPoint.Y].IsPath = true;
                    pointX = lowestPoint.X;
                    pointY = lowestPoint.Y;
                }
                else
                {
                    break;
                }

                if (_squares[pointX, pointY].ContentCode == SquareContent.Boris) //We went from monster to hero, so we're finished.
                {
                    break;
                }
            }
        }

        public Point OffsetPoint(Point apoint)
        {
            return new Point(apoint.X + gridOffset.X, apoint.Y + gridOffset.Y);
            return apoint;
        }

        public Vector2 OffsetPosition(Vector2 apos)
        {
            return new Vector2(apos.X + (tileSize * gridOffset.X), apos.Y + (tileSize * gridOffset.Y));
        }


        //gets location is world coordinates of the grid point
        public Vector2 ReturnTilePos(Point thePoint)
        {
          //  borisPos = OffsetPosition(borisPos);
           // thePoint = OffsetPoint(thePoint);
            return new Vector2(thePoint.X * tileSize, thePoint.Y * tileSize);
        }

        //Reset every square
        public void ClearSquares()
        {
            foreach (Point point in AllSquares())
            {
                _squares[point.X, point.Y] = new CompleteSquare();
            }
        }

        //Reset some information about the squares.
        public void ClearLogic()
        {
            foreach (Point point in AllSquares())
            {
                int x = point.X;
                int y = point.Y;
                _squares[x, y].DistanceSteps = 10000;
                _squares[x, y].IsPath = false;
            }
        }

        // Return every point on the board in order.
        private static IEnumerable<Point> AllSquares()
        {
            for (int x = 0; x < gridSize.X; x++)
            {
                for (int y = 0; y < gridSize.Y; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        // Find the requested code and return the point.
        private Point FindCode(SquareContent contentIn)
        {
            foreach (Point point in AllSquares())
            {
                if (_squares[point.X, point.Y].ContentCode == contentIn)
                {
                    return new Point(point.X, point.Y);
                }
            }
            return new Point(-1, -1);
        }

        // A square is open if it is not a wall.
        private bool SquareOpen(int x, int y)
        {
            switch (_squares[x, y].ContentCode)
            {
                case SquareContent.Empty:
                    return true;
                case SquareContent.Boris:
                    return true;
                case SquareContent.Destination:
                    return true;
                case SquareContent.Wall:
                    if (ignorewalls == false)
                    {
                        return false;
                    }
                    else
                    {
                    
                        return true;
                    }
                default:
                    return false;
            }
        }

        //Return each valid square we can move to.
        private IEnumerable<Point> ValidMoves(int x, int y)
        {
            foreach (Point movePoint in _movements)
            {
                int newX = x + movePoint.X;
                int newY = y + movePoint.Y;

                if (ValidCoordinates(newX, newY) &&
                    SquareOpen(newX, newY))
                {
                    if (canDiagonal(x, y, movePoint.X, movePoint.Y))
                    {
                        yield return new Point(newX, newY);
                    }
                }
            }
        }

        //Our coordinates are constrained between 0 and 14.
        static private bool ValidCoordinates(int x, int y)
        {
            if (x < 0)
            {
                return false;
            }
            if (y < 0)
            {
                return false;
            }
            if (x > gridSize.X - 1)
            {
                return false;
            }
            if (y > gridSize.Y - 1)
            {
                return false;
            }
            return true;
        }

        //Check that taking diagonal path will not cause boris to walk through objects
        public bool canDiagonal(int x, int y, int movepointX, int movepointY)
        {
            if (movepointX != 0 && movepointY != 0)
            {
                if ((SquareOpen(x, movepointY + y)) && (SquareOpen(movepointX + x, y)))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        //can be used for moving cursor
        public void HandleInput(InputState input)
        {
            PlayerIndex index;
            if (input.IsMenuUp(null))
            {
                this.MoveCursor(new Point(0, -1));
            }
            if (input.IsMenuDown(null))
            {
                this.MoveCursor(new Point(0, 1));
            }
            if (input.IsMenuLeft(null))
            {
                this.MoveCursor(new Point(-1, 0));
            }
            if (input.IsMenuRight(null))
            {
                this.MoveCursor(new Point(1, 0));
            }
            if (input.IsMenuSelect2(null))
            {
                this.ToggleWallTile();
            }
        }

        //will place or remove a wall tile at the cursor's position, then update the path
        public void ToggleWallTile()
        {
            if (_squares[cursorPos.X, cursorPos.Y].ContentCode == SquareContent.Empty)
            {
                _squares[cursorPos.X, cursorPos.Y].ContentCode = SquareContent.Wall;
            }
            else if (_squares[cursorPos.X, cursorPos.Y].ContentCode == SquareContent.Wall)
            {
                _squares[cursorPos.X, cursorPos.Y].ContentCode = SquareContent.Empty;
            }
            else
            {
                _squares[cursorPos.X, cursorPos.Y].ContentCode = SquareContent.Empty;
            }

            this.ClearLogic();
            try
            {
                this.Pathfind(false);
       
            }
            catch
            {
                //   toolStripStatusLabel1.Text = "IO Error";
            }
            finally
            {
                this.UpdateTileColors();
            }
        }

        //moves pink cross around the board for selecting squares/quads
        public void MoveCursor(Point movement)
        {
            cursorPos = new Point(movement.X + cursorPos.X, movement.Y + cursorPos.Y);

            if (cursorPos.X > gridSize.X - 1)
                cursorPos.X = 0;
            if (cursorPos.Y > gridSize.Y - 1)
                cursorPos.Y = 0;
            if (cursorPos.X < 0)
                cursorPos.X = gridSize.X - 1;
            if (cursorPos.Y < 0)
                cursorPos.Y = gridSize.Y - 1;

            Vector2 newpos = ReturnTilePos(cursorPos);
            cursorTile.tilePosition = new Vector3(newpos.X, cursorTile.tilePosition.Y, newpos.Y);
            cursorTile2.tilePosition = new Vector3(newpos.X, cursorTile2.tilePosition.Y, newpos.Y);
        }

        public void UpdateTileColors()
        {
            foreach (Point point in AllSquares())
            {
                int x = point.X;
                int y = point.Y;
                int num = _squares[x, y].DistanceSteps;
                Color setColor = Color.Gainsboro;
                SquareContent content = _squares[x, y].ContentCode;

                if (content == SquareContent.Empty)
                {
                    if (_squares[x, y].IsPath == true)
                    {
                        setColor = Color.LightBlue;
                    }
                    else
                    {
                        setColor = Color.White;
                    }
                }
                else
                {
                    if (content == SquareContent.Boris)
                    {
                        setColor = Color.Green;
                        CurrentTilePos = new Point(x, y);
                    }
                    else if (content == SquareContent.Destination)
                    {
                        setColor = Color.Coral;
                    }
                    else
                    {
                        setColor = Color.Gray;
                    }
                }
                theTiles[x, y].theQuad.changeColor(setColor);
            }
        }


        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            for (int i = 0; i < gridSize.X; i++)
            {
                for (int j = 0; j < gridSize.Y; j++)
                {
                    theTiles[i, j].Draw(device, view, projection);
                }
            }
            cursorTile.Draw(device, view, projection);
            cursorTile2.Draw(device, view, projection);
            Quad quad2 = new Quad(Vector3.Zero, Vector3.Up, Vector3.Up, 100.0f, 100.0f);
            BasicEffect quadEffect = new BasicEffect(device);
            quadEffect.View = view;
            quadEffect.Projection = projection;

            quadEffect.World = Matrix.CreateTranslation(new Vector3(0, 5, 0));
            quadEffect.AmbientLightColor = new Vector3(0.2f, 1.0f, 0.2f);
        }
    }
}
