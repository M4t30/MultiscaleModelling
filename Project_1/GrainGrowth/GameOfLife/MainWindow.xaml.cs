using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        int squaresInWidth = 300;
        int squaresInHeight = 300;
        int grainSize = 6;
        int numberAlive = 100;
        int numberOfInclusions = 0;
        int xMin, xMax, yMin, yMax;
        int actualNumberOfInclusions = 0;
        int sizeOfInclusions = 0;
        int addedAmount = 0;
        int groundBoundarySize = 0;
        int mooreProbability = 0;
        bool onGrainBounds = false;
        bool squareInclusion = false;
        bool radialInclusion = false;
        bool vonNeumann = false;
        bool moore = false;
        bool monteCarlo = false;
        bool monteCarloRandom = false;
        bool random = false;
        bool uniform = false;
        bool randomWithRadius = false;
        bool periodic = false;
        bool addRandom = false;
        bool IsSub = false;
        public string inputFileName;
        public string outputFileName;
        bool[,] wasDrawed = new bool[30, 30];
        string path = "";
        Brush aBrushBlack = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        Brush aBrushDP = new SolidColorBrush(Color.FromRgb(140, 140, 140));
        struct Cell
        {
            public int x, y;
            public Brush aBrush;
            public string id;
            public int energy;
            public Cell(int x, int y, Brush aBrush, string id = "", int energy = 0)
            {
                this.x = x;
                this.y = y;
                this.aBrush = aBrush;
                this.id = id;
                this.energy = energy;
            }
        };

        List<Cell> cells = new List<Cell>();
        List<string> grainsSelectedToDPBrush = new List<string>();
        List<Cell> selectedGrainsToDP = new List<Cell>();
        List<Color> colors = new List<Color>();
        List<int> colorPower = new List<int>();
        List<Cell> selectedGrains = new List<Cell>();
        List<string> selectedGrainsGBBrush = new List<string>();
        Rectangle[,] grid;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            grid = new Rectangle[squaresInHeight, squaresInWidth];
            InitializeComponent();
            gridArea.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            gridArea.Arrange(new Rect(0.0, 0.0, gridArea.DesiredSize.Width, gridArea.DesiredSize.Height));
            timer.Interval = TimeSpan.FromSeconds(0.0000001);
            timer.Tick += Timer_Tick;
            gridArea.Height = 600.0f;
            gridArea.Width = 600.0f;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            seedGrowing();
        }

        private void seedGrowing()
        {
            if (vonNeumann == true)
                calculateVonNeumann();

            if (moore == true)
                calculateMoore();

            if (monteCarlo == true)
                calculateMonteCarlo();
        }

        private void calculatePeriodicIndexes(ref int indexAbove, ref int indexBelow, ref int indexLeft, ref int indexRight)
        {
            if (indexAbove < 0) { indexAbove = squaresInHeight - 1; }
            if (indexBelow >= squaresInHeight) { indexBelow = 0; }
            if (indexLeft < 0) { indexLeft = squaresInWidth - 1; }
            if (indexRight >= squaresInWidth) { indexRight = 0; }
        }

        private void calculateNonPeriodicIndexes(ref int indexAbove, ref int indexBelow, ref int indexLeft, ref int indexRight)
        {
            if (indexAbove < 0) { indexAbove = -1; }
            if (indexBelow >= squaresInHeight) { indexBelow = -1; }
            if (indexLeft < 0) { indexLeft = -1; }
            if (indexRight >= squaresInWidth) { indexRight = -1; }
        }

        private void calculateVonNeumann()
        {
            cells.Clear();

            Dictionary<Brush, int> Power = new Dictionary<Brush, int>();

            for (int i = 0; i < squaresInHeight; i++)
            {
                for (int j = 0; j < squaresInWidth; j++)
                {
                    int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                    if (periodic == true)
                        calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                    else
                        calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                    if (grid[i, j].Fill.ToString() == Brushes.White.ToString())
                    {
                        if (indexAbove != -1 && grid[indexAbove, j].Fill.ToString() != Brushes.White.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushBlack.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexAbove, j].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexAbove, j].Fill))
                                Power[grid[indexAbove, j].Fill] += 1;

                            else
                                Power.Add(grid[indexAbove, j].Fill, 1);
                        }

                        if (indexLeft != -1 && grid[i, indexLeft].Fill.ToString() != Brushes.White.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushBlack.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[i, indexLeft].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[i, indexLeft].Fill))
                                Power[grid[i, indexLeft].Fill] += 1;

                            else
                                Power.Add(grid[i, indexLeft].Fill, 1);
                        }

                        if (indexRight != -1 && grid[i, indexRight].Fill.ToString() != Brushes.White.ToString() && grid[i, indexRight].Fill.ToString() != aBrushBlack.ToString() && grid[i, indexRight].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[i, indexRight].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[i, indexRight].Fill))
                                Power[grid[i, indexRight].Fill] += 1;

                            else
                                Power.Add(grid[i, indexRight].Fill, 1);
                        }

                        if (indexBelow != -1 && grid[indexBelow, j].Fill.ToString() != Brushes.White.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushBlack.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexBelow, j].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexBelow, j].Fill))
                                Power[grid[indexBelow, j].Fill] += 1;

                            else
                                Power.Add(grid[indexBelow, j].Fill, 1);
                        }

                        if (Power.Count > 0)
                        {
                            var key = Power.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                            cells.Add(new Cell(i, j, key));
                            Power.Clear();
                        }
                    }
                }
            }

            for (int i = 0; i < cells.Count; i++)
                grid[cells[i].x, cells[i].y].Fill = cells[i].aBrush;
        }

        private void calculateMoore()
        {
            Random aRandom = new Random();
            cells.Clear();
            bool rule4 = false;
            Dictionary<Brush, int> Power = new Dictionary<Brush, int>();
            
            for (int i = 0; i < squaresInHeight; i++)
            {
                for (int j = 0; j < squaresInWidth; j++)
                {
                    rule4 = true;
                    Power.Clear();

                    int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                    if (periodic == true)
                        calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                    else
                        calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                    if (grid[i, j].Fill.ToString() == Brushes.White.ToString())
                    {
                        if (indexAbove != -1 && grid[indexAbove, j].Fill.ToString() != Brushes.White.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushBlack.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexAbove, j].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexAbove, j].Fill))
                                Power[grid[indexAbove, j].Fill] += 1;

                            else
                                Power.Add(grid[indexAbove, j].Fill, 1);
                        }

                        if (indexLeft != -1 && indexAbove != -1 && grid[indexAbove, indexLeft].Fill.ToString() != Brushes.White.ToString() && grid[indexAbove, indexLeft].Fill.ToString() != aBrushBlack.ToString() && grid[indexAbove, indexLeft].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexAbove, indexLeft].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexAbove, indexLeft].Fill))
                                Power[grid[indexAbove, indexLeft].Fill] += 1;

                            else
                                Power.Add(grid[indexAbove, indexLeft].Fill, 1);
                        }

                        if (indexRight != -1 && indexAbove != -1 && grid[indexAbove, indexRight].Fill.ToString() != Brushes.White.ToString() && grid[indexAbove, indexRight].Fill.ToString() != aBrushBlack.ToString() && grid[indexAbove, indexRight].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexAbove, indexRight].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexAbove, indexRight].Fill))
                                Power[grid[indexAbove, indexRight].Fill] += 1;

                            else
                                Power.Add(grid[indexAbove, indexRight].Fill, 1);
                        }

                        if (indexLeft != -1 && grid[i, indexLeft].Fill.ToString() != Brushes.White.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushBlack.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[i, indexLeft].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[i, indexLeft].Fill))
                                Power[grid[i, indexLeft].Fill] += 1;

                            else
                                Power.Add(grid[i, indexLeft].Fill, 1);
                        }

                        if (indexRight != -1 && grid[i, indexRight].Fill.ToString() != Brushes.White.ToString() && grid[i, indexRight].Fill.ToString() != aBrushBlack.ToString() && grid[i, indexRight].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[i, indexRight].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[i, indexRight].Fill))
                                Power[grid[i, indexRight].Fill] += 1;

                            else
                                Power.Add(grid[i, indexRight].Fill, 1);
                        }

                        if (indexBelow != -1 && grid[indexBelow, j].Fill.ToString() != Brushes.White.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushBlack.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexBelow, j].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexBelow, j].Fill))
                                Power[grid[indexBelow, j].Fill] += 1;

                            else
                                Power.Add(grid[indexBelow, j].Fill, 1);
                        }

                        if (indexBelow != -1 && indexLeft != -1 && grid[indexBelow, indexLeft].Fill.ToString() != Brushes.White.ToString() && grid[indexBelow, indexLeft].Fill.ToString() != aBrushBlack.ToString() && grid[indexBelow, indexLeft].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexBelow, indexLeft].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexBelow, indexLeft].Fill))
                                Power[grid[indexBelow, indexLeft].Fill] += 1;

                            else
                                Power.Add(grid[indexBelow, indexLeft].Fill, 1);
                        }

                        if (indexBelow != -1 && indexRight != -1 && grid[indexBelow, indexRight].Fill.ToString() != Brushes.White.ToString() && grid[indexBelow, indexRight].Fill.ToString() != aBrushBlack.ToString() && grid[indexBelow, indexRight].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexBelow, indexRight].Fill.ToString()))
                        {
                            if (Power.ContainsKey(grid[indexBelow, indexRight].Fill))
                                Power[grid[indexBelow, indexRight].Fill] += 1;

                            else
                                Power.Add(grid[indexBelow, indexRight].Fill, 1);
                        }

                        Dictionary<Brush, int> InitialPower = new Dictionary<Brush, int>(Power);

                        if (Power.Count > 0)
                        {
                            var key = Power.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                            int val = Power.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
                            if ( val >= 5)
                            {
                                cells.Add(new Cell(i, j, key));
                                Power.Clear();
                                rule4 = false;
                            }

                            else // checking nearest neighbours
                            {                              
                                Power.Clear();

                                if (indexAbove != -1 && grid[indexAbove, j].Fill.ToString() != Brushes.White.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushBlack.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexAbove, j].Fill.ToString()))
                                {
                                    if (Power.ContainsKey(grid[indexAbove, j].Fill))
                                        Power[grid[indexAbove, j].Fill] += 1;

                                    else
                                        Power.Add(grid[indexAbove, j].Fill, 1);
                                }

                                if (indexLeft != -1 && grid[i, indexLeft].Fill.ToString() != Brushes.White.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushBlack.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[i, indexLeft].Fill.ToString()))
                                {
                                    if (Power.ContainsKey(grid[i, indexLeft].Fill))
                                        Power[grid[i, indexLeft].Fill] += 1;

                                    else
                                        Power.Add(grid[i, indexLeft].Fill, 1);
                                }

                                if (indexRight != -1 && grid[i, indexRight].Fill.ToString() != Brushes.White.ToString() && grid[i, indexRight].Fill.ToString() != aBrushBlack.ToString() && grid[i, indexRight].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[i, indexRight].Fill.ToString()))
                                {
                                    if (Power.ContainsKey(grid[i, indexRight].Fill))
                                        Power[grid[i, indexRight].Fill] += 1;

                                    else
                                        Power.Add(grid[i, indexRight].Fill, 1);
                                }

                                if (indexBelow != -1 && grid[indexBelow, j].Fill.ToString() != Brushes.White.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushBlack.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexBelow, j].Fill.ToString()))
                                {
                                    if (Power.ContainsKey(grid[indexBelow, j].Fill))
                                        Power[grid[indexBelow, j].Fill] += 1;

                                    else
                                        Power.Add(grid[indexBelow, j].Fill, 1);
                                }

                                if (Power.Count > 0)
                                {
                                    var keyNearest = Power.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                                    int valNearest = Power.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;

                                    if (valNearest >= 3)
                                    {
                                        cells.Add(new Cell(i, j, keyNearest));
                                        Power.Clear();
                                        rule4 = false;
                                    }

                                    else // checking further grains
                                    {
                                        Power.Clear();

                                        if (indexLeft != -1 && indexAbove != -1 && grid[indexAbove, indexLeft].Fill.ToString() != Brushes.White.ToString() && grid[indexAbove, indexLeft].Fill.ToString() != aBrushBlack.ToString() && grid[indexAbove, indexLeft].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexAbove, indexLeft].Fill.ToString()))
                                        {
                                            if (Power.ContainsKey(grid[indexAbove, indexLeft].Fill))
                                                Power[grid[indexAbove, indexLeft].Fill] += 1;

                                            else
                                                Power.Add(grid[indexAbove, indexLeft].Fill, 1);
                                        }

                                        if (indexRight != -1 && indexAbove != -1 && grid[indexAbove, indexRight].Fill.ToString() != Brushes.White.ToString() && grid[indexAbove, indexRight].Fill.ToString() != aBrushBlack.ToString() && grid[indexAbove, indexRight].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexAbove, indexRight].Fill.ToString()))
                                        {
                                            if (Power.ContainsKey(grid[indexAbove, indexRight].Fill))
                                                Power[grid[indexAbove, indexRight].Fill] += 1;

                                            else
                                                Power.Add(grid[indexAbove, indexRight].Fill, 1);
                                        }

                                        if (indexBelow != -1 && indexLeft != -1 && grid[indexBelow, indexLeft].Fill.ToString() != Brushes.White.ToString() && grid[indexBelow, indexLeft].Fill.ToString() != aBrushBlack.ToString() && grid[indexBelow, indexLeft].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexBelow, indexLeft].Fill.ToString()))
                                        {
                                            if (Power.ContainsKey(grid[indexBelow, indexLeft].Fill))
                                                Power[grid[indexBelow, indexLeft].Fill] += 1;

                                            else
                                                Power.Add(grid[indexBelow, indexLeft].Fill, 1);
                                        }

                                        if (indexBelow != -1 && indexRight != -1 && grid[indexBelow, indexRight].Fill.ToString() != Brushes.White.ToString() && grid[indexBelow, indexRight].Fill.ToString() != aBrushBlack.ToString() && grid[indexBelow, indexRight].Fill.ToString() != aBrushDP.ToString() && !grainsSelectedToDPBrush.Contains(grid[indexBelow, indexRight].Fill.ToString()))
                                        {
                                            if (Power.ContainsKey(grid[indexBelow, indexRight].Fill))
                                                Power[grid[indexBelow, indexRight].Fill] += 1;

                                            else
                                                Power.Add(grid[indexBelow, indexRight].Fill, 1);
                                        }

                                        if (Power.Count > 0)
                                        {
                                            var keyFurthest = Power.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                                            int valFurthest = Power.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
                                            if (valFurthest >= 3)
                                            {
                                                cells.Add(new Cell(i, j, keyFurthest));
                                                Power.Clear();
                                                rule4 = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (InitialPower.Count > 0 && rule4)
                        {
                           
                            int initVal = aRandom.Next(0, 100);

                            if (initVal < mooreProbability)
                            {
                                var lastKey = InitialPower.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                                cells.Add(new Cell(i, j, lastKey));
                            }
                        }

                        Power.Clear();
                        InitialPower.Clear();
                        rule4 = true;
                    }
                }
            }

            for (int i = 0; i < cells.Count; i++)
                grid[cells[i].x, cells[i].y].Fill = cells[i].aBrush;
        }

        private void calculateMonteCarlo()
        {
            for (int i = 0; i < squaresInHeight; i++)
                for (int j = 0; j < squaresInWidth; j++)
                    wasDrawed[i, j] = false;

            int drawedAmount = 0;

            Random x = new Random();

            while (drawedAmount < (squaresInHeight * squaresInWidth))
            {
                int i = x.Next(0, squaresInHeight);
                int j = x.Next(0, squaresInWidth);

                if (wasDrawed[i, j] == false)
                {
                    int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                    if (periodic == true)
                        calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                    else
                        calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                    Random r = new Random();
                    Brush secondBrush = new SolidColorBrush(colors[r.Next(0, colors.Count)]);

                    int firstEnergy = countEnergy(indexAbove, indexBelow, indexLeft, indexRight, i, j, grid[i, j].Fill);
                    int secondEnergy = countEnergy(indexAbove, indexBelow, indexLeft, indexRight, i, j, secondBrush);

                    if (firstEnergy >= secondEnergy)
                        grid[i, j].Fill = secondBrush;

                    wasDrawed[i, j] = true;
                    drawedAmount++;
                }
            }
        }

        private int countEnergy(int indexAbove, int indexBelow, int indexLeft, int indexRight, int i, int j, Brush aBrush)
        {
            int energy = 8;

            Brush aBrushTwo;

            if (indexAbove >= 0 && indexLeft >= 0)
            {
                aBrushTwo = grid[indexAbove, indexLeft].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            if (indexAbove >= 0 && j >= 0)
            {
                aBrushTwo = grid[indexAbove, j].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            if (indexAbove >= 0 && indexRight >= 0)
            {
                aBrushTwo = grid[indexAbove, indexRight].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            if (i >= 0 && indexLeft >= 0)
            {
                aBrushTwo = grid[i, indexLeft].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            if (i >= 0 && indexRight >= 0)
            {
                aBrushTwo = grid[i, indexRight].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            if (indexBelow >= 0 && indexLeft >= 0)
            {
                aBrushTwo = grid[indexBelow, indexLeft].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            if (indexBelow >= 0 && j >= 0)
            {
                aBrushTwo = grid[indexBelow, j].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            if (indexBelow >= 0 && indexRight >= 0)
            {
                aBrushTwo = grid[indexBelow, indexRight].Fill;
                if (aBrush.ToString() == aBrushTwo.ToString()) energy--;
            }

            return energy;
        }

        private void drawGrid()
        {
            gridArea.Width = squaresInWidth * grainSize;
            gridArea.Height = squaresInHeight * grainSize;

            for (int i = 0; i < squaresInHeight; i++)
            {
                for (int j = 0; j < squaresInWidth; j++)
                {
                    Rectangle aRectangle = new Rectangle();
                    aRectangle.Width = grainSize;
                    aRectangle.Height = grainSize;
                    aRectangle.Fill = Brushes.White;
                    gridArea.Children.Add(aRectangle);

                    Canvas.SetLeft(aRectangle, j * aRectangle.Width);
                    Canvas.SetTop(aRectangle, i * aRectangle.Height);
                    aRectangle.MouseDown += R_MouseDown;
                    grid[i, j] = aRectangle;
                }
            }

            if (random == true)
                randomGridGenerator();

            if (uniform == true)
                uniformGridGenerator();

            if (randomWithRadius == true)
                randomWithRadiusGridGenerator();

            if (monteCarloRandom == true)
                monteCarloRandomGridGenerator();

            /* for (int i = 0; i < squaresInHeight; i++)
             {
                 for (int j = 0; j < squaresInWidth; j++)
                 {
                     Rectangle aRectangle = new Rectangle();
                     aRectangle.Width = gridArea.ActualWidth / squaresInWidth;
                     aRectangle.Height = gridArea.ActualHeight / squaresInHeight;

                     if (aRectangle.Width != aRectangle.Height)
                     {
                         if (aRectangle.Width > aRectangle.Height)
                         {
                             aRectangle.Height = aRectangle.Width;
                         }

                         else
                         {
                             aRectangle.Width = aRectangle.Height;
                         }
                     }

                     if (aRectangle.Width < GrainSize)
                         aRectangle.Width = GrainSize;

                     if (aRectangle.Height < GrainSize)
                         aRectangle.Height = GrainSize;

                     aRectangle.Fill = Brushes.White;

                     if (gridArea.Height < i * aRectangle.Height)
                     {
                         gridArea.Height += GrainSize;
                     }

                     if (gridArea.Width < j * aRectangle.Width)
                     {
                         gridArea.Width += GrainSize;
                     }

                     gridArea.Children.Add(aRectangle);

                     Canvas.SetLeft(aRectangle, j * aRectangle.Width);
                     Canvas.SetTop(aRectangle, i * aRectangle.Height);
                     aRectangle.MouseDown += R_MouseDown;
                     grid[i, j] = aRectangle;
                 }
             }

             if (random == true)
                 randomGridGenerator();

             if (uniform == true)
                 uniformGridGenerator();

             if (randomWithRadius == true)
                 randomWithRadiusGridGenerator();

             if (monteCarloRandom == true)
                 monteCarloRandomGridGenerator();*/
        }

        private void randomGridGenerator()
        {
            Random rectangleChooser = new Random();
            int aliveCounter = 0;

            while (aliveCounter < numberAlive)
            {
                int xRectangle = rectangleChooser.Next(0, squaresInHeight);
                int yRectangle = rectangleChooser.Next(0, squaresInWidth);
                if (grid[xRectangle, yRectangle].Fill == Brushes.White)
                {
                    Brush aBrush = new SolidColorBrush(colors[aliveCounter]);
                    grid[xRectangle, yRectangle].Fill = aBrush;
                    aliveCounter++;
                }
            }
        }

        private void uniformGridGenerator()
        {
            if (numberAlive == 1)
            {
                Brush aBrush = new SolidColorBrush(colors[0]);
                grid[squaresInWidth / 2, squaresInHeight / 2].Fill = aBrush;
            }

            else
            {
                int aliveCounter = 0;

                int gap = (squaresInHeight * squaresInWidth) / numberAlive;

                if (numberAlive % squaresInWidth == 0)
                {
                    gap = squaresInHeight / (numberAlive / 10) + squaresInWidth;
                }

                int index1D = gap;
                Brush aBrush = new SolidColorBrush(colors[aliveCounter]);
                grid[0, 0].Fill = aBrush;
                aliveCounter++;

                while (aliveCounter < numberAlive)
                {
                    int indexX = index1D / squaresInWidth;
                    int indexY = index1D % squaresInHeight;
                    aBrush = new SolidColorBrush(colors[aliveCounter]);
                    grid[indexX, indexY].Fill = aBrush;
                    aliveCounter++;
                    index1D += gap;
                }
            }
        }

        private void randomWithRadiusGridGenerator()
        {
            if (((numberAlive * (2 * sizeOfInclusions * sizeOfInclusions + 6 * sizeOfInclusions + 1)) > (squaresInHeight * squaresInWidth)) || (sizeOfInclusions > squaresInHeight) || (sizeOfInclusions > squaresInWidth))
                MessageBox.Show("Radius is too big!");

            else
            {
                Random rectangleChooser = new Random();
                int aliveCounter = 0;

                while (aliveCounter < numberAlive)
                {
                    int xRectangle = rectangleChooser.Next(0, squaresInHeight);
                    int yRectangle = rectangleChooser.Next(0, squaresInWidth);
                    if ((grid[xRectangle, yRectangle].Fill == Brushes.White) && isEnoughSpaceRadius(xRectangle, yRectangle))
                    {
                        Brush aBrush = new SolidColorBrush(colors[aliveCounter]);
                        grid[xRectangle, yRectangle].Fill = aBrush;
                        aliveCounter++;
                    }
                }
            }
        }

        private void monteCarloRandomGridGenerator()
        {
            Random rectangleChooser = new Random();
            Random r = new Random();
            int aliveCounter = 0;

            while (aliveCounter < squaresInHeight * squaresInWidth)
            {
                int xRectangle = rectangleChooser.Next(0, squaresInHeight);
                int yRectangle = rectangleChooser.Next(0, squaresInWidth);
                if (grid[xRectangle, yRectangle].Fill == Brushes.White)
                {
                    Brush aBrush = new SolidColorBrush(colors[r.Next(0, colors.Count)]);
                    grid[xRectangle, yRectangle].Fill = aBrush;
                    aliveCounter++;
                }
            }
        }

        private bool isEnoughSpaceRadius(int x, int y)
        {
            if (x - sizeOfInclusions < 0 || x + sizeOfInclusions > (squaresInWidth - 1) || y - sizeOfInclusions < 0 || y + sizeOfInclusions > (squaresInHeight - 1))
            {
                return false;
            }

            else
            {
                for (int i = y - sizeOfInclusions + 1; i < y + sizeOfInclusions; i++)
                {
                    for (int j = x; j > x - sizeOfInclusions; j--)
                    {
                        if (i == y && j == x)
                            continue;

                        if (grid[j, i].Fill.ToString() == aBrushBlack.ToString())
                        {
                            return false;
                        }
                    }

                    for (int j = x + 1; j < x + sizeOfInclusions; j++)
                    {
                        if (i == y && j == x)
                            continue;

                        if (grid[j, i].Fill.ToString() == aBrushBlack.ToString())
                        {
                            return false;
                        }
                    }
                }

                if (grid[x, y].Fill.ToString() == aBrushBlack.ToString())
                {
                    return false;
                }
            }

            return true;
        }

        private bool isEnoughSpaceSquare(int x, int y)
        {
            if (periodic == true)
            {
                if (x - (sizeOfInclusions - 1) < 0) xMin = squaresInWidth - (sizeOfInclusions - 1) + x; else xMin = x - (sizeOfInclusions - 1);
                if (x > squaresInWidth) xMax = (x) % squaresInWidth; else xMax = x;
                if (y - (sizeOfInclusions - 1) < 0) yMin = squaresInHeight - (sizeOfInclusions - 1) + y; else yMin = y - (sizeOfInclusions - 1);
                if (y > squaresInHeight) yMax = (y) % squaresInHeight; else yMax = y;

                for (int i = xMin; i != (xMax + 1) % squaresInWidth; i = ((i + 1) % squaresInWidth))
                {
                    for (int j = yMin; j != (yMax + 1) % squaresInHeight; j = ((j + 1) % squaresInHeight))
                    {
                        if (grid[i, j].Fill.ToString() == aBrushBlack.ToString())
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            else//NONPeriodic
            {
                xMin = x - (sizeOfInclusions - 1);
                xMax = x;
                yMin = y - (sizeOfInclusions - 1);
                yMax = y;

                for (int i = xMin; i < xMax + 1; i++)
                {
                    for (int j = yMin; j < yMax + 1; j++)
                    {
                        if ((i >= 0 && i < squaresInWidth) && (j >= 0 && j < squaresInHeight))
                        {
                            if (grid[i, j].Fill.ToString() == aBrushBlack.ToString())
                            {
                                return false;
                            }
                        }

                        else
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        void setInputTxtFileName()
        {
            inputFileName = path;
            inputFileName += "\\";
            inputFileName += inputFileNameTextBox.Text;
            inputFileName += ".txt";
        }

        void setOutputTxtFileName()
        {
            outputFileName = path;
            outputFileName += "\\";
            outputFileName += outputFileNameTextBox.Text;
            outputFileName += ".txt";
        }

        void setInputBmpFileName()
        {
            inputFileName = path;
            inputFileName += "\\";
            inputFileName += inputFileNameTextBox.Text;
            inputFileName += ".bmp";
        }

        void setOutputBmpFileName()
        {
            outputFileName = path;
            outputFileName += "\\";
            outputFileName += outputFileNameTextBox.Text;
            outputFileName += ".bmp";
        }

        public void readTxtFile()
        {
            setInputTxtFileName();

            using (TextReader reader = File.OpenText(inputFileName))
            {
                string line;

                line = reader.ReadLine();

                string[] size = line.Split(' ');
                squaresInWidth = Int32.Parse(size[0]);
                squaresInHeight = Int32.Parse(size[1]);

                grid = new Rectangle[squaresInHeight, squaresInWidth];
                gridArea.Background = new SolidColorBrush(Colors.White);

                gridArea.Height = squaresInHeight * grainSize;
                gridArea.Width = squaresInWidth * grainSize;

                int HeightIndex = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] brushes = line.Split(' ');

                    for (int j = 0; j < squaresInWidth; j++)
                    {
                        Rectangle aRectangle = new Rectangle();
                        aRectangle.Width = grainSize;
                        aRectangle.Height = grainSize;

                        aRectangle.Fill = new BrushConverter().ConvertFromString(brushes[j]) as SolidColorBrush;

                        gridArea.Children.Add(aRectangle);

                        Canvas.SetLeft(aRectangle, j * aRectangle.Width);
                        Canvas.SetTop(aRectangle, HeightIndex * aRectangle.Height);
                        aRectangle.MouseDown += R_MouseDown;
                        grid[HeightIndex, j] = aRectangle;
                    }

                    HeightIndex++;
                }

                reader.Close();
            }
        }

        public void writeToTxtFile()
        {
            setOutputTxtFileName();

            using (TextWriter writer = File.CreateText(outputFileName))
            {
                writer.WriteLine("{0} {1}", squaresInWidth, squaresInHeight);

                for (int i = 0; i < squaresInHeight; i++)
                {
                    for (int j = 0; j < squaresInWidth; j++)
                    {
                        writer.Write("{0} ", grid[i, j].Fill);
                    }

                    writer.WriteLine();
                }

                writer.Close();
            }
        }

        public void writeToBmpFile()
        {
            setOutputBmpFileName();

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)gridArea.Width, (int)gridArea.Height, 96d, 96d, PixelFormats.Pbgra32);
            gridArea.Measure(new Size((int)gridArea.Width, (int)gridArea.Height));
            gridArea.Arrange(new Rect(new Size((int)gridArea.Width, (int)gridArea.Height)));
            renderBitmap.Render(gridArea);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(outputFileName))
            {
                encoder.Save(file);
            }
        }

        public void readBmpFile()
        {
            setInputBmpFileName();

            cells.Clear();

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(inputFileName, UriKind.Relative));
            gridArea.Height = brush.ImageSource.Height;
            gridArea.Width = brush.ImageSource.Width;
            squaresInWidth = Convert.ToInt32(gridArea.Width / Convert.ToDouble(grainSize));
            squaresInHeight = Convert.ToInt32(gridArea.Height / Convert.ToDouble(grainSize));
            gridArea.Background = brush;
            grid = new Rectangle[squaresInWidth, squaresInHeight];

            float halfGrainSize = grainSize / 2.0f;
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)gridArea.Width, (int)gridArea.Height, 96d, 96d, PixelFormats.Pbgra32);
            gridArea.Measure(new Size((int)gridArea.Width, (int)gridArea.Height));
            gridArea.Arrange(new Rect(new Size((int)gridArea.Width, (int)gridArea.Height)));
            renderTargetBitmap.Render(gridArea);

            for (int i = 0; i < squaresInWidth; i++)
            {
                for (int j = 0; j < squaresInHeight; j++)
                {
                    Rectangle aRectangle = new Rectangle();
                    aRectangle.Width = grainSize;
                    aRectangle.Height = grainSize;

                    float locationX = halfGrainSize + (i * grainSize);
                    float locationY = halfGrainSize + (j * grainSize);

                    if ((locationX <= renderTargetBitmap.PixelWidth) && (locationY <= renderTargetBitmap.PixelHeight))
                    {
                        var croppedBitmap = new CroppedBitmap(renderTargetBitmap, new Int32Rect((int)locationX, (int)locationY, 1, 1));
                        var pixels = new byte[4];
                        croppedBitmap.CopyPixels(pixels, 4, 0);
                        Color pixelColor = Color.FromRgb(pixels[2], pixels[1], pixels[0]);
                        aRectangle.Fill = new SolidColorBrush(pixelColor);
                        gridArea.Children.Add(aRectangle);
                        Canvas.SetLeft(aRectangle, i * aRectangle.Width);
                        Canvas.SetTop(aRectangle, j * aRectangle.Height);
                        aRectangle.MouseDown += R_MouseDown;
                        grid[i, j] = aRectangle;
                    }
                }
            }
        }

        private void drawCircle(int xc, int yc, int x, int y)
        {
            grid[xc + x, yc + y].Fill = aBrushBlack;
            grid[xc - x, yc + y].Fill = aBrushBlack;
            grid[xc + x, yc - y].Fill = aBrushBlack;
            grid[xc - x, yc - y].Fill = aBrushBlack;
            grid[xc + y, yc + x].Fill = aBrushBlack;
            grid[xc - y, yc + x].Fill = aBrushBlack;
            grid[xc + y, yc - x].Fill = aBrushBlack;
            grid[xc - y, yc - x].Fill = aBrushBlack;
        }

        private void circleBres(int xc, int yc, int r)
        {
            int x = 0, y = r;
            int d = 3 - 2 * r;
            drawCircle(xc, yc, x, y);
            while (y >= x)
            {
                // for each pixel we will
                // draw all eight pixels
                x++;
                // check for decision parameter
                // and correspondingly
                // update d, x, y
                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                    d = d + 4 * x + 6;
                drawCircle(xc, yc, x, y);
            }
        }

        private void fillCircle(int xc, int yc, int r)
        {
            for (int i = yc - r + 1; i < yc + r; i++)
            {
                for (int j = xc; j > xc - r; j--)
                {
                    if (i == yc && j == xc)
                        continue;

                    if (grid[j, i].Fill.ToString() == aBrushBlack.ToString())
                    {
                        break;
                    }

                    grid[j, i].Fill = aBrushBlack;
                }

                for (int j = xc + 1; j < xc + r; j++)
                {
                    if (i == yc && j == xc)
                        continue;

                    if (grid[j, i].Fill.ToString() == aBrushBlack.ToString())
                    {
                        break;
                    }

                    grid[j, i].Fill = aBrushBlack;
                }
            }

            grid[xc, yc].Fill = aBrushBlack;
        }

        private void addInclusions()
        {
            if (numberOfInclusions != 0)
            {
                Random x = new Random();

                while (actualNumberOfInclusions < numberOfInclusions)
                {
                    int i = x.Next(0, squaresInWidth);
                    int j = x.Next(0, squaresInHeight);

                    if (onGrainBounds)
                    {
                        if (grid[i, j].Fill != aBrushBlack)
                        {
                            int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                            if (periodic == true)
                                calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            else
                                calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            if (indexLeft != -1 && grid[i, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);
                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }

                                continue;
                            }

                            if (indexLeft != -1 && indexAbove != -1 && grid[indexAbove, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, indexLeft].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);

                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }
                                continue;
                            }

                            if (indexAbove != -1 && grid[indexAbove, j].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);

                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }
                                continue;
                            }

                            if (indexAbove != -1 && indexRight != -1 && grid[indexAbove, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, indexRight].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);

                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }
                                continue;
                            }

                            if (indexRight != -1 && grid[i, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[i, indexRight].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);
                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }
                                continue;
                            }

                            if (indexBelow != -1 && indexRight != -1 && grid[indexBelow, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, indexRight].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);
                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }
                                continue;
                            }

                            if (indexBelow != -1 && grid[indexBelow, j].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);
                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }
                                continue;
                            }

                            if (indexBelow != -1 && indexLeft != -1 && grid[indexBelow, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, indexLeft].Fill.ToString() != aBrushBlack.ToString())
                            {
                                if (sizeOfInclusions > 1)
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        for (int k = xMin; k < xMax + 1; k++)
                                        {
                                            for (int l = yMin; l < yMax + 1; l++)
                                            {
                                                if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                    grid[k, l].Fill = aBrushBlack;
                                            }
                                        }

                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        circleBres(i, j, sizeOfInclusions);
                                        fillCircle(i, j, sizeOfInclusions);
                                        actualNumberOfInclusions++;
                                    }
                                }

                                else
                                {
                                    if (squareInclusion && isEnoughSpaceSquare(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }

                                    if (radialInclusion && isEnoughSpaceRadius(i, j))
                                    {
                                        grid[i, j].Fill = aBrushBlack;
                                        actualNumberOfInclusions++;
                                    }
                                }

                                continue;
                            }
                        }
                    }

                    else
                    {
                        if (sizeOfInclusions > 1)
                        {
                            if (squareInclusion && isEnoughSpaceSquare(i, j))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                actualNumberOfInclusions++;
                            }

                            if (radialInclusion && isEnoughSpaceRadius(i, j))
                            {
                                circleBres(i, j, sizeOfInclusions);
                                fillCircle(i, j, sizeOfInclusions);
                                actualNumberOfInclusions++;
                            }
                        }

                        else
                        {
                            if (squareInclusion && isEnoughSpaceSquare(i, j))
                            {
                                grid[i, j].Fill = aBrushBlack;
                                actualNumberOfInclusions++;
                            }

                            if (radialInclusion && isEnoughSpaceRadius(i, j))
                            {
                                grid[i, j].Fill = aBrushBlack;
                                actualNumberOfInclusions++;
                            }
                        }
                    }
                }
            }

            else
            {
                MessageBox.Show("Increase Number Of Inclusions");
            }
        }

        private void drawGroundBoundary()
        {
            if (groundBoundarySize > 1)
            {
                for (int i = 0; i < squaresInWidth; i++)
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (grid[i, j].Fill != aBrushBlack)
                        {
                            int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                            if (periodic == true)
                                calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            else
                                calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            xMin = i - (groundBoundarySize - 1);
                            xMax = i;
                            yMin = j - (groundBoundarySize - 1);
                            yMax = j;

                            if (indexLeft != -1 && grid[i, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushBlack.ToString())
                            {                               
                                    for (int k = xMin; k < xMax + 1; k++)
                                    {
                                        for (int l = yMin; l < yMax + 1; l++)
                                        {
                                            if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                grid[k, l].Fill = aBrushBlack;
                                        }
                                    }                                  
                                
                                continue;
                            }

                            if (indexLeft != -1 && indexAbove != -1 && grid[indexAbove, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, indexLeft].Fill.ToString() != aBrushBlack.ToString())
                            {                               
                                    for (int k = xMin; k < xMax + 1; k++)
                                    {
                                        for (int l = yMin; l < yMax + 1; l++)
                                        {
                                            if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                grid[k, l].Fill = aBrushBlack;
                                        }
                                    }
                                                                    
                                continue;
                            }

                            if (indexAbove != -1 && grid[indexAbove, j].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushBlack.ToString())
                            {                               
                                    for (int k = xMin; k < xMax + 1; k++)
                                    {
                                        for (int l = yMin; l < yMax + 1; l++)
                                        {
                                            if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                grid[k, l].Fill = aBrushBlack;
                                        }
                                    }
                                                                   
                                continue;
                            }

                            if (indexAbove != -1 && indexRight != -1 && grid[indexAbove, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, indexRight].Fill.ToString() != aBrushBlack.ToString())
                            {                              
                                    for (int k = xMin; k < xMax + 1; k++)
                                    {
                                        for (int l = yMin; l < yMax + 1; l++)
                                        {
                                            if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                grid[k, l].Fill = aBrushBlack;
                                        }
                                    }
                                                                                                
                                continue;
                            }

                            if (indexRight != -1 && grid[i, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[i, indexRight].Fill.ToString() != aBrushBlack.ToString())
                            {                              
                                    for (int k = xMin; k < xMax + 1; k++)
                                    {
                                        for (int l = yMin; l < yMax + 1; l++)
                                        {
                                            if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                grid[k, l].Fill = aBrushBlack;
                                        }
                                    }
                                                                  
                                continue;
                            }

                            if (indexBelow != -1 && indexRight != -1 && grid[indexBelow, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, indexRight].Fill.ToString() != aBrushBlack.ToString())
                            {                             
                                    for (int k = xMin; k < xMax + 1; k++)
                                    {
                                        for (int l = yMin; l < yMax + 1; l++)
                                        {
                                            if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                grid[k, l].Fill = aBrushBlack;
                                        }
                                    }
                                                                
                                continue;
                            }

                            if (indexBelow != -1 && grid[indexBelow, j].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushBlack.ToString())
                            {                               
                                    for (int k = xMin; k < xMax + 1; k++)
                                    {
                                        for (int l = yMin; l < yMax + 1; l++)
                                        {
                                            if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                                grid[k, l].Fill = aBrushBlack;
                                        }
                                    }
                                                                
                                continue;
                            }

                            if (indexBelow != -1 && indexLeft != -1 && grid[indexBelow, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, indexLeft].Fill.ToString() != aBrushBlack.ToString())
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }
                        }
                    }
                }
                             
            }

            else
            {
                for (int i = 0; i < squaresInWidth; i++) // horizontal
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (grid[i, j].Fill != aBrushBlack)
                        {
                            int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                            if (periodic == true)
                                calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            else
                                calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            if (indexLeft != -1 && grid[i, indexLeft].Fill != grid[i, j].Fill && grid[i, indexLeft].Fill != aBrushBlack)
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }

                            if (indexRight != -1 && grid[i, indexRight].Fill != grid[i, j].Fill && grid[i, indexRight].Fill != aBrushBlack)
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }
                        }
                    }
                }

                for (int i = 0; i < squaresInWidth; i++) //vertical
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (grid[i, j].Fill != aBrushBlack)
                        {
                            int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                            if (periodic == true)
                                calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            else
                                calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            if (indexAbove != -1 && grid[indexAbove, j].Fill != grid[i, j].Fill && grid[indexAbove, j].Fill != aBrushBlack)
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }

                            if (indexBelow != -1 && grid[indexBelow, j].Fill != grid[i, j].Fill && grid[indexBelow, j].Fill != aBrushBlack)
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }
                        }
                    }
                }
            }

            SetGBPercentage();
        }

        private void drawSGGroundBoundary()
        {
            if (groundBoundarySize > 1)
            {
                for (int i = 0; i < squaresInWidth; i++)
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (grid[i, j].Fill != aBrushBlack)
                        {
                            int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                            if (periodic == true)
                                calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            else
                                calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            xMin = i - (groundBoundarySize - 1);
                            xMax = i;
                            yMin = j - (groundBoundarySize - 1);
                            yMax = j;

                            if (indexLeft != -1 && grid[i, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[i, indexLeft].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[i, indexLeft].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }

                            if (indexLeft != -1 && indexAbove != -1 && grid[indexAbove, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, indexLeft].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[indexAbove, indexLeft].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }

                            if (indexAbove != -1 && grid[indexAbove, j].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, j].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[indexAbove, j].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }

                            if (indexAbove != -1 && indexRight != -1 && grid[indexAbove, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexAbove, indexRight].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[indexAbove, indexRight].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }

                            if (indexRight != -1 && grid[i, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[i, indexRight].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[i, indexRight].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }

                            if (indexBelow != -1 && indexRight != -1 && grid[indexBelow, indexRight].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, indexRight].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[indexBelow, indexRight].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }

                            if (indexBelow != -1 && grid[indexBelow, j].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, j].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[indexBelow, j].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }

                            if (indexBelow != -1 && indexLeft != -1 && grid[indexBelow, indexLeft].Fill.ToString() != grid[i, j].Fill.ToString() && grid[indexBelow, indexLeft].Fill.ToString() != aBrushBlack.ToString() && selectedGrainsGBBrush.Contains(grid[indexBelow, indexLeft].Fill.ToString()))
                            {
                                for (int k = xMin; k < xMax + 1; k++)
                                {
                                    for (int l = yMin; l < yMax + 1; l++)
                                    {
                                        if ((k >= 0 && k < squaresInWidth) && (l >= 0 && l < squaresInHeight))
                                            grid[k, l].Fill = aBrushBlack;
                                    }
                                }

                                continue;
                            }
                        }
                    }
                }

            }

            else
            {
                for (int i = 0; i < squaresInWidth; i++) // horizontal
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (grid[i, j].Fill != aBrushBlack)
                        {
                            int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                            if (periodic == true)
                                calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            else
                                calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            if (indexLeft != -1 && grid[i, indexLeft].Fill != grid[i, j].Fill && grid[i, indexLeft].Fill != aBrushBlack && selectedGrainsGBBrush.Contains(grid[i, indexLeft].Fill.ToString()))
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }

                            if (indexRight != -1 && grid[i, indexRight].Fill != grid[i, j].Fill && grid[i, indexRight].Fill != aBrushBlack && selectedGrainsGBBrush.Contains(grid[i, indexRight].Fill.ToString()))
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }
                        }
                    }
                }

                for (int i = 0; i < squaresInWidth; i++) //vertical
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (grid[i, j].Fill != aBrushBlack)
                        {
                            int indexAbove = i - 1, indexBelow = i + 1, indexLeft = j - 1, indexRight = j + 1;

                            if (periodic == true)
                                calculatePeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            else
                                calculateNonPeriodicIndexes(ref indexAbove, ref indexBelow, ref indexLeft, ref indexRight);

                            if (indexAbove != -1 && grid[indexAbove, j].Fill != grid[i, j].Fill && grid[indexAbove, j].Fill != aBrushBlack && selectedGrainsGBBrush.Contains(grid[indexAbove, j].Fill.ToString()))
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }

                            if (indexBelow != -1 && grid[indexBelow, j].Fill != grid[i, j].Fill && grid[indexBelow, j].Fill != aBrushBlack && selectedGrainsGBBrush.Contains(grid[indexBelow, j].Fill.ToString()))
                            {
                                grid[i, j].Fill = aBrushBlack;
                            }
                        }
                    }
                }
            }

            SetGBPercentage();
        }

        private void SetGBPercentage()
        {
            int GBcellsNumber = 0;

            for (int i = 0; i < squaresInWidth; i++)
            {
                for (int j = 0; j < squaresInHeight; j++)
                {
                    if (grid[i,j].Fill.ToString() == aBrushBlack.ToString())
                    {
                        GBcellsNumber++;
                    }
                }
            }

            float a = GBcellsNumber;
            float b = squaresInHeight;
            float c = squaresInWidth;

            float percentage = (a / (b * c)) * 100.0f;
            labelGBPercentage.Content = percentage.ToString();
        }

        private bool isInclusionClose(int i, int j, int indexAbove, int indexBelow, int indexLeft, int indexRight)
        {
            bool isInclusionNear = false;

            if (indexLeft != -1 && grid[i, indexLeft].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            if (indexLeft != -1 && indexAbove != -1 && grid[indexAbove, indexLeft].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            if (indexAbove != -1 && grid[indexAbove, j].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            if (indexAbove != -1 && indexRight != -1 && grid[indexAbove, indexRight].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            if (indexRight != -1 && grid[i, indexRight].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            if (indexBelow != -1 && indexRight != -1 && grid[indexBelow, indexRight].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            if (indexBelow != -1 && grid[indexBelow, j].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            if (indexBelow != -1 && indexLeft != -1 && grid[indexBelow, indexLeft].Fill == aBrushBlack)
            {
                isInclusionNear = true;
            }

            return isInclusionNear;
        }

        private void selectAllGrains()
        {
            selectedGrains.Clear();

            for (int i = 0; i < squaresInWidth; i++)
            {
                for (int j = 0; j <squaresInWidth; j++)
                {
                    if (grid[i, j].Fill != aBrushBlack && grid[i, j].Fill != Brushes.White)
                    {
                        selectedGrains.Add(new Cell(i, j, grid[i, j].Fill, grid[i,j].ToString(), 0));                     
                    }
                }
            }
        }

        private void clearNotSelectedGrainsGB()
        {
            selectedGrains.Clear();
            selectedGrainsGBBrush.Add(aBrushBlack.ToString());

            for (int i = 0; i < squaresInWidth; i++)
            {
                for (int j = 0; j < squaresInHeight; j++)
                {
                    if (!selectedGrainsGBBrush.Contains(grid[i,j].Fill.ToString()))
                    {
                        selectedGrains.Add(new Cell(i, j, grid[i, j].Fill, grid[i, j].ToString(), 0));
                    }
                }
            }         
        }

        private void eraseSelectedGrains()
        {
            for (int i = 0; i < selectedGrains.Count; i++)
            {
                grid[selectedGrains[i].x, selectedGrains[i].y].Fill = Brushes.White; 
            }
        }

        private void clearGrainsToDP()
        {
            grainsSelectedToDPBrush.Clear();
        }

        private void selectAllGrainsToDP()
        {
            selectedGrainsToDP.Clear();
            
            if (IsSub)
            {
                grainsSelectedToDPBrush.Add(aBrushDP.ToString());
                for (int i = 0; i < squaresInWidth; i++)
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (!grainsSelectedToDPBrush.Contains(grid[i, j].Fill.ToString()))
                        {
                            selectedGrainsToDP.Add(new Cell(i, j, grid[i, j].Fill, grid[i, j].Fill.ToString(), 0));
                        }
                    }
                }
            }

            else
            {
                colors.Add(Color.FromRgb(140, 140, 140));

                for (int i = 0; i < squaresInWidth; i++)
                {
                    for (int j = 0; j < squaresInHeight; j++)
                    {
                        if (grainsSelectedToDPBrush.Contains(grid[i, j].Fill.ToString()))
                        {
                            //grainsSelectedToDPBrush.Remove(grid[i, j].Fill.ToString());
                            SolidColorBrush b = grid[i, j].Fill as SolidColorBrush;
                            colors.Remove(b.Color);
                            grid[i, j].Fill = aBrushDP;
                        }

                        if (grid[i,j].Fill.ToString() != aBrushDP.ToString())
                        {
                            selectedGrainsToDP.Add(new Cell(i, j, grid[i, j].Fill, grid[i, j].Fill.ToString(), 0));
                        }
                    }
                }
            }
        }

        private void eraseNoDPGrains()
        {
            for (int i = 0; i < selectedGrainsToDP.Count; i++)
            {
                grid[selectedGrainsToDP[i].x, selectedGrainsToDP[i].y].Fill = Brushes.White;
            }
        }

        private void R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grainsSelectedToDPBrush.Add(((Rectangle)sender).Fill.ToString());
            selectedGrainsGBBrush.Add(((Rectangle)sender).Fill.ToString());
        }

        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                buttonStartStop.Content = "Start animation";
            }

            else
            {
                timer.Start();
                buttonStartStop.Content = "Stop animation";
            }
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < squaresInWidth; i++)
                for (int j = 0; j < squaresInHeight; j++)
                    grid[i, j].Fill = Brushes.White;

            colors.Clear();
            colorPower.Clear();

            numberAlive = Convert.ToInt32(textBox.Text);
            Random r = new Random();

            int counter = 0;

            while (counter < numberAlive)
            {
                int redInt = r.Next(1, 254);
                var red = Convert.ToByte(redInt);
                int greenInt = r.Next(1, 254);
                var green = Convert.ToByte(greenInt);
                int blueInt = r.Next(1, 254);
                var blue = Convert.ToByte(blueInt);
                var tmpColor = Color.FromRgb(red, green, blue);
                if (!colors.Contains(tmpColor) && red != 140 && blue != 140 && green != 140)
                {
                    counter++;
                    colors.Add(Color.FromRgb(red, green, blue));
                }
            }
        }

        private void buttonDrawGrid_Click(object sender, RoutedEventArgs e)
        {
            grid = new Rectangle[squaresInHeight, squaresInWidth];
            gridArea.Background = new SolidColorBrush(Colors.White);
            drawGrid();
        }

        private void textBoxAliveNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            colors.Clear();
            colorPower.Clear();
            if (textBox.Text.ToString() == "")
            {
                numberAlive = 0;
            }

            else
            {
                numberAlive = Convert.ToInt32(textBox.Text);
                Random r = new Random();

                int counter = 0;

                while (counter < numberAlive)
                {
                    int redInt = r.Next(1, 254);
                    var red = Convert.ToByte(redInt);
                    int greenInt = r.Next(1, 254);
                    var green = Convert.ToByte(greenInt);
                    int blueInt = r.Next(1, 254);
                    var blue = Convert.ToByte(blueInt);
                    var tmpColor = Color.FromRgb(red, green, blue);
                    if (!colors.Contains(tmpColor) && red != 140 && blue != 140 && green != 140)
                    {
                        counter++;
                        colors.Add(Color.FromRgb(red, green, blue));
                    }
                }
            }
        }

        private void textBoxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox1.Text.ToString() == "")
                squaresInWidth = 0;

            else
            {
                squaresInWidth = Convert.ToInt32(textBox1.Text);
                wasDrawed = new bool[squaresInHeight, squaresInWidth];
            }
        }

        private void textBoxHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox2.Text.ToString() == "")
                squaresInHeight = 0;

            else
            {
                squaresInHeight = Convert.ToInt32(textBox2.Text);
                wasDrawed = new bool[squaresInHeight, squaresInWidth];
            }
        }

        private void ListBoxItemVonNewmann(object sender, RoutedEventArgs e)
        {
            vonNeumann = true;
            moore = false;
            monteCarlo = false;
        }

        private void ListBoxItemMoore(object sender, RoutedEventArgs e)
        {
            vonNeumann = false;
            moore = true;
            monteCarlo = false;
        }

        private void ListBoxItemRandom(object sender, RoutedEventArgs e)
        {
            random = true;
            uniform = false;
            randomWithRadius = false;
            addRandom = false;
            monteCarloRandom = false;
        }

        private void ListBoxItemUniform(object sender, RoutedEventArgs e)
        {
            random = false;
            uniform = true;
            randomWithRadius = false;
            addRandom = false;
            monteCarloRandom = false;
        }

        private void ListBoxItemRandomWithRadius(object sender, RoutedEventArgs e)
        {
            random = false;
            uniform = false;
            randomWithRadius = true;
            addRandom = false;
            monteCarloRandom = false;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            periodic = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            periodic = false;
        }

        private void ListBoxItemMonteCarlo(object sender, RoutedEventArgs e)
        {
            vonNeumann = false;
            moore = false;
            monteCarlo = true;
        }

        private void AddRandom(object sender, RoutedEventArgs e)
        {
            random = false;
            uniform = false;
            randomWithRadius = false;
            addRandom = true;
            monteCarloRandom = false;
        }

        private void textBoxAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxAdd.Text.ToString() == "")
                addedAmount = 0;

            else
                addedAmount = Convert.ToInt32(textBoxAdd.Text);
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            Random rectangleChooser = new Random();
            int aliveCounter = 0;

            while (aliveCounter < addedAmount)
            {
                int xRectangle = rectangleChooser.Next(0, squaresInHeight);
                int yRectangle = rectangleChooser.Next(0, squaresInWidth);
                if (grid[xRectangle, yRectangle].Fill == Brushes.White)
                {
                    Random r = new Random();
                    int counter = 0;

                    while (counter < numberAlive)
                    {
                        int redInt = r.Next(1, 254);
                        var red = Convert.ToByte(redInt);
                        int greenInt = r.Next(1, 254);
                        var green = Convert.ToByte(greenInt);
                        int blueInt = r.Next(1, 254);
                        var blue = Convert.ToByte(blueInt);
                        var tmpColor = Color.FromRgb(red, green, blue);
                        if (!colors.Contains(tmpColor) && red != 140 && blue != 140 && green != 140)
                        {
                            counter++;
                            colors.Add(Color.FromRgb(red, green, blue));
                        }
                    }

                    Brush aBrush = new SolidColorBrush(colors[numberAlive]);
                    grid[xRectangle, yRectangle].Fill = aBrush;
                    numberAlive++;
                    aliveCounter++;
                }
            }
        }

        private void ListBoxItemMonteCarloRandom(object sender, RoutedEventArgs e)
        {
            monteCarloRandom = true;
            random = false;
            uniform = false;
            randomWithRadius = false;
            addRandom = false;
        }

        private void buttonSaveFileTXT_Click(object sender, RoutedEventArgs e)
        {
            SetFolderPath();
            writeToTxtFile();
        }

        private void buttonLoadFileTXT_Click(object sender, RoutedEventArgs e)
        {
            SetFolderPath();
            readTxtFile();
        }

        private void textBoxNumberOfInclusions_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxAmountOfInclusions.Text.ToString() == "")
                numberOfInclusions = 0;

            else
                numberOfInclusions = Convert.ToInt32(textBoxAmountOfInclusions.Text);
        }

        private void textBoxSizeOfInclusions_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxSizeOfInclusions.Text.ToString() == "")
                sizeOfInclusions = 0;

            else
                sizeOfInclusions = Convert.ToInt32(textBoxSizeOfInclusions.Text);
        }

        private void ListBoxItemSquareInclusion(object sender, RoutedEventArgs e)
        {
            squareInclusion = true;
            radialInclusion = false;
        }

        private void ListBoxItemRadialInclusion(object sender, RoutedEventArgs e)
        {
            squareInclusion = false;
            radialInclusion = true;
        }

        private void textBoxGroundBoundarySize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxGroundBoundarySize.Text.ToString() == "")
                groundBoundarySize = 0;

            else
                groundBoundarySize = Convert.ToInt32(textBoxGroundBoundarySize.Text);
        }

        private void buttonDrawGroundBoundary_Click(object sender, RoutedEventArgs e)
        {
            drawGroundBoundary();
        }

        private void buttonSelectAllGrains_Click(object sender, RoutedEventArgs e)
        {
            selectAllGrains();
        }

        private void buttonEraseSelectedGrains_Click(object sender, RoutedEventArgs e)
        {
            eraseSelectedGrains();
        }

        private void buttonClearGrainsToDP_Click(object sender, RoutedEventArgs e)
        {
            clearGrainsToDP();
        }

        private void buttonSelectAllGrainsToDP_Click(object sender, RoutedEventArgs e)
        {
            selectAllGrainsToDP();
        }

        private void buttonEraseNoDPGrains_Click(object sender, RoutedEventArgs e)
        {
            eraseNoDPGrains();
        }

        private void buttonSaveFileBMP_Click(object sender, RoutedEventArgs e)
        {
            SetFolderPath();
            writeToBmpFile();
        }

        private void buttonLoadFileBMP_Click(object sender, RoutedEventArgs e)
        {
            SetFolderPath();
            readBmpFile();
        }

        private void buttonAddInclusions_Click(object sender, RoutedEventArgs e)
        {
            addInclusions();
        }

        private void textBoxMooreProbability_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxMooreProbability.Text.ToString() == "")
                mooreProbability = 0;

            else
                mooreProbability = Convert.ToInt32(textBoxMooreProbability.Text);
        }

        private void checkBoxIsSub_Checked(object sender, RoutedEventArgs e)
        {
            IsSub = true;
        }

        private void checkBoxIsSub_Unchecked(object sender, RoutedEventArgs e)
        {
            IsSub = false;
        }

        private void buttonSelectNotClickedGrainsGB_Click(object sender, RoutedEventArgs e)
        {
            clearNotSelectedGrainsGB();
        }

        private void buttonDrawSGGroundBoundary_Click(object sender, RoutedEventArgs e)
        {
            drawSGGroundBoundary();
        }

        private void checkBoxBounds_Checked(object sender, RoutedEventArgs e)
        {
            onGrainBounds = true;
        }

        private void checkBoxBounds_Unchecked(object sender, RoutedEventArgs e)
        {
            onGrainBounds = false;
        }

        private void SetFolderPath()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                path = dialog.SelectedPath;
            }
        }
    }
}
