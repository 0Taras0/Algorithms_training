using Logic;
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

            int cx = (int)GraphCanvas.ActualWidth / 2;
            int cy = (int)GraphCanvas.ActualHeight / 2;
            int r = Math.Min(cx, cy) - 40;

            if (n == 1)
                NodePositions[nodes[0]] = new Point(cx, cy);
            else
            {
                for (int i = 0; i < n; i++)
                {
                    double angle = i * 2 * Math.PI / n;
                    double x = cx + r * Math.Cos(angle);
                    double y = cy + r * Math.Sin(angle);

                    x = Math.Max(40, Math.Min(x, GraphCanvas.ActualWidth - 40));
                    y = Math.Max(40, Math.Min(y, GraphCanvas.ActualHeight - 40));

                    NodePositions[nodes[i]] = new Point(x, y);
                }
            }
        }
        void DrawNode(int id, Point p)
        {
            Ellipse el = new()
            {
                Width = 40,
                Height = 40,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(el, p.X - 20);
            Canvas.SetTop(el, p.Y - 20);

            GraphCanvas.Children.Add(el);

            TextBlock t = new()
            {
                Text = id.ToString(),
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(t, p.X - 8);
            Canvas.SetTop(t, p.Y - 10);

            GraphCanvas.Children.Add(t);
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

            var result = g.BFS(start, end);
            ResultTextBox.Text = "Результат BFS: " + string.Join(", ", result);
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

            var result = g.DFS(start, end);
            ResultTextBox.Text = "Результат DFS: " + string.Join(", ", result);
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