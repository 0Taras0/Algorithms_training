using Logic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Graph g = new();
        Dictionary<int, Point> NodePositions = new();
        double NodeSize = 40;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void IsDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigit(e.Text);
        }

        private bool IsDigit(string input)
        {
            return input.All(char.IsDigit);
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EdgeInput.Text) || !EdgeInput.Text.Contains("-"))
            {
                MessageBox.Show("Будь ласка, введіть правильне ребро у форматі: 'початкова-кінець'");
                return;
            }

            var p = EdgeInput.Text.Split('-');

            if (int.TryParse(p[0], out int a) && int.TryParse(p[1], out int b))
            {
                g.AddEdge(a, b);
                DrawGraph();
                EdgeInput.Clear();
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть правильні числа для ребра.");
            }
        }

        private void EdgeInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AddEdge_Click(sender, e);
        }

        void DrawGraph()
        {
            GraphCanvas.Children.Clear();
            AutoPlaceNodes();

            foreach (var u in g.adj.Keys)
                foreach (var v in g.adj[u])
                    DrawEdge(NodePositions[u], NodePositions[v]);

            foreach (var n in NodePositions)
                DrawNode(n.Key, n.Value);
        }

        void AutoPlaceNodes()
        {
            NodePositions.Clear();
            var nodes = g.GetNodes();
            int n = nodes.Count;
            if (n <= 20)
                NodeSize = 40;
            else if (n <= 200)
                NodeSize = 25;
            else if (n <= 1000)
                NodeSize = 12;
            else if (n <= 5000)
                NodeSize = 6;
            else
                NodeSize = 3;

            int cx = (int)GraphCanvas.ActualWidth / 2;
            int cy = (int)GraphCanvas.ActualHeight / 2;
            int r = Math.Min(cx, cy) - 40;

            if (n == 1)
            {
                NodePositions[nodes[0]] = new Point(cx, cy);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    double angle = i * 2 * Math.PI / n;
                    double x = cx + r * Math.Cos(angle);
                    double y = cy + r * Math.Sin(angle);

                    x = Math.Max(NodeSize, Math.Min(x, GraphCanvas.ActualWidth - NodeSize));
                    y = Math.Max(NodeSize, Math.Min(y, GraphCanvas.ActualHeight - NodeSize));

                    NodePositions[nodes[i]] = new Point(x, y);
                }
            }
        }

        void DrawNode(int id, Point p)
        {
            double r = NodeSize / 2;

            Ellipse el = new()
            {
                Width = NodeSize,
                Height = NodeSize,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = NodeSize < 8 ? 0.5 : 1
            };

            Canvas.SetLeft(el, p.X - r);
            Canvas.SetTop(el, p.Y - r);
            GraphCanvas.Children.Add(el);

            if (NodeSize >= 15)
            {
                TextBlock t = new()
                {
                    Text = id.ToString(),
                    FontSize = NodeSize * 0.5,
                    FontWeight = FontWeights.Bold
                };

                Canvas.SetLeft(t, p.X - (NodeSize * 0.2));
                Canvas.SetTop(t, p.Y - (NodeSize * 0.3));

                GraphCanvas.Children.Add(t);
            }
        }

        void DrawEdge(Point a, Point b)
        {
            Line line = new()
            {
                X1 = a.X,
                Y1 = a.Y,
                X2 = b.X,
                Y2 = b.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            GraphCanvas.Children.Add(line);
        }

        private void BFSButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StartNodeSearch.Text))
            {
                MessageBox.Show("Будь ласка, введіть початкову вершину для BFS.");
                return;
            }

            int start = int.Parse(StartNodeSearch.Text);
            int? end = null;

            if (int.TryParse(EndNodeSearch.Text, out int endNode))
            {
                end = endNode;
            }

            var sw = Stopwatch.StartNew();
            var result = g.BFS(start, end);
            sw.Stop();

            ResultTextBox.AppendText("Результат BFS: " + string.Join(", ", result) + "\n");
            ResultTextBox.AppendText($"Час BFS: {sw.Elapsed.TotalMilliseconds} мс\n\n");
            ResultTextBox.ScrollToEnd();
        }

        private void DFSButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StartNodeSearch.Text))
            {
                MessageBox.Show("Будь ласка, введіть початкову вершину для DFS.");
                return;
            }

            int start = int.Parse(StartNodeSearch.Text);
            int? end = null;

            if (int.TryParse(EndNodeSearch.Text, out int endNode))
            {
                end = endNode;
            }

            var sw = Stopwatch.StartNew();
            var result = g.DFS(start, end);
            sw.Stop();

            ResultTextBox.AppendText("Результат DFS: " + string.Join(", ", result) + "\n");
            ResultTextBox.AppendText($"Час DFS: {sw.Elapsed.TotalMilliseconds} мс\n\n");
            ResultTextBox.ScrollToEnd();
        }

        private void ClearGraph_Click(object sender, RoutedEventArgs e)
        {
            g = new Graph();
            NodePositions.Clear();
            GraphCanvas.Children.Clear();
            ResultTextBox.Clear();
        }
    }
}