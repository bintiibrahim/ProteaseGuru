﻿using SharpLearning.DecisionTrees.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProteaseGuruGUI
{
    class SequenceCoverageMap
    {
        public static void Highlight(int start, int end, Canvas map, Dictionary<int, List<int>> indices, int height, Color clr, bool unique, bool partial = false)
        {
            int spacing = 22;
            int increment = 0;
            int i;
            //int increment = indices.Where(index => index == start || index == end).Count() * 5;

            for (i = 0; i < indices.Count; ++i)
            {
                if (!indices[i].Any(d => d == start))
                {
                    break;
                }

                increment += 5;
            }

            // update list of indices
            if (indices.ContainsKey(i))
            {
                indices[i].AddRange(Enumerable.Range(start, end - start + 1));
            }
            else
            {
                indices.Add(i, Enumerable.Range(start, end - start + 1).ToList());
            }

            // highlight peptides
            if (partial) // continue highlight peptides from prev line
            {
                peptideLineDrawing(map, new Point((start- 0.7) * spacing + 10, height + increment), new Point((start - 0.5) * spacing + 10, height + increment), Colors.Red, false);
            }

            if (unique)
            {
                peptideLineDrawing(map, new Point(start * spacing + 10, height + increment), new Point(end * spacing + 10, height + increment), clr, false);
            }
            else
            {
                peptideLineDrawing(map, new Point(start * spacing + 10, height + increment), new Point(end * spacing + 10, height + increment), clr, true);
            }
        }

        public static void txtDrawing(Canvas cav, Point loc, string txt, Brush clr)
        {
            TextBlock tb = new TextBlock();
            tb.Foreground = clr;
            tb.Text = txt;
            tb.FontSize = 15;
            tb.FontWeight = FontWeights.Bold;
            tb.FontFamily = new FontFamily("Arial"); // monospaced font

            Canvas.SetTop(tb, loc.Y);
            Canvas.SetLeft(tb, loc.X);
            Panel.SetZIndex(tb, 2); //lower priority
            cav.Children.Add(tb);
            cav.UpdateLayout();
        }

        // draw line for peptides
        public static void peptideLineDrawing(Canvas cav, Point start, Point end, Color clr, bool shared)
        {
            // draw top
            Line top = new Line();
            top.Stroke = new SolidColorBrush(clr);
            top.X1 = start.X;
            top.X2 = end.X + 11;
            top.Y1 = start.Y + 20;
            top.Y2 = end.Y + 20;
            top.StrokeThickness = 2;

            if (shared)
            {
                top.StrokeDashArray = new DoubleCollection() { 2 };
            }

            cav.Children.Add(top);

            Canvas.SetZIndex(top, 1); //on top of any other things in canvas
        }

        public static void drawLegend(Canvas cav, Dictionary<string, Color> proteaseByColor, string[] proteases, Grid legend)
        {
            int i = -1;
            foreach (var protease in proteases)
            {
                legend.ColumnDefinitions.Add(new ColumnDefinition());
                legend.ColumnDefinitions.Add(new ColumnDefinition());
                Label proteaseName = new Label();
                proteaseName.Content = protease;

                Rectangle proteaseColor = new Rectangle();
                proteaseColor.Fill = new SolidColorBrush(proteaseByColor[protease]);
                proteaseColor.Width = 30;
                proteaseColor.Height = 15;

                legend.Children.Add(proteaseColor);
                Grid.SetColumn(proteaseColor, ++i);
                legend.Children.Add(proteaseName);
                Grid.SetColumn(proteaseName, ++i);
            }

            legend.ColumnDefinitions.Add(new ColumnDefinition());
            legend.ColumnDefinitions.Add(new ColumnDefinition());
            legend.ColumnDefinitions.Add(new ColumnDefinition());
            legend.ColumnDefinitions.Add(new ColumnDefinition());

            string[] peptides = new string[2] { "Shared", "Unique" };
            foreach (string peptide in peptides)
            {
                Line pepLine = new Line();
                pepLine.X1 = 0;
                pepLine.X2 = 50;
                pepLine.Y1 = 0;
                pepLine.Y2 = 0;
                pepLine.StrokeThickness = 1;
                pepLine.Stroke = Brushes.Black;
                pepLine.HorizontalAlignment = HorizontalAlignment.Center;
                pepLine.VerticalAlignment = VerticalAlignment.Center;

                Label pepLabel = new Label();
                pepLabel.Content = peptide + " peptides";

                if (peptide.Equals("Shared"))
                {
                    pepLine.StrokeDashArray = new DoubleCollection() { 2 };
                }

                legend.Children.Add(pepLine);
                legend.Children.Add(pepLabel);
                Grid.SetColumn(pepLine, ++i);
                Grid.SetColumn(pepLabel, ++i);
            }

            cav.Visibility = Visibility.Visible;
        }
    }

    class ProteinForSeqCoverage
    {
        public ProteinForSeqCoverage(string accession, string map, double fraction)
        {
            Accession = accession;
            Map = map;
            Fraction = fraction;
        }

        public string Accession { get; set; }
        public string Map { get; set; }
        public double Fraction { get; set; }
    }
}
