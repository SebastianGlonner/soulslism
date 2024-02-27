namespace EfesusProfiler;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    static class GraphColors
    {
        static private Color SYSTEM_LINE = new Color(0, 0, 0);
        static private Color[] GRAPH_LINE = new Color[] {
            new Color(127, 0, 0),
            new Color(0, 127, 0),
            new Color(127, 127, 0),
        };

        static public Color system()
        {
            return SYSTEM_LINE;
        }
        static public Color graph(int index)
        {
            return GRAPH_LINE[index];
        }
    }

