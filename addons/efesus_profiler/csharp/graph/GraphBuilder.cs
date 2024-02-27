namespace EfesusProfiler;
    
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;


public partial class GraphBuilder : Control
    {
        static private Color SYSTEM_LINE = new Color(0, 0, 0);
        static private Color GRAPH_LINE = new Color(127, 0, 0);
        static private Color GRAPH_LINE2 = new Color(0, 127, 0);

        // scroll container
        private int width = 400;
        private int height = 300;

        // label container
        private int labelContainerWidth = 30;

        private int marginBottom = 13; // space for the scroll bar

        private ScrollContainer scrollGraph;
        private ProfilerMetricsViewerGraph graph;
        private LegendY legendY;

        public override void _Ready()
        {
            ControlUtil.ApplyDefaults(this);
            //this.OffsetTop = height;
            //this.OffsetRight = width;


            scrollGraph = GetTree().Root.FindChild("ScrollGraph", true, false) as ScrollContainer;
            //ControlUtil.ApplyAnchorFull(scrollGraph);
            scrollGraph.CustomMinimumSize = new Vector2(width, height);

            graph = ControlUtil.FindNode<ProfilerMetricsViewerGraph>(GetTree());

            legendY = ControlUtil.FindNode<LegendY>(GetTree());
            legendY.CustomMinimumSize = new Vector2(labelContainerWidth, height);
            legendY.color = SYSTEM_LINE;

            graph.MaxYChange += Graph_MaxYChange;

            setProjection(0);
        }

        private void Graph_MaxYChange(object sender, MaxYChange e)
        {
            legendY.maxYChanged(e.maxY);
            setProjection(e.maxY);
        }

        private void setProjection(float maxValue)
        {
            graph.projection = new Projection(height - marginBottom, maxValue);
            legendY.projection = new Projection(height - marginBottom, maxValue);

        }
    }
