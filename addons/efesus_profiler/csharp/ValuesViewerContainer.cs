namespace EfesusProfiler;
using Godot;
using System;
using System.Collections.Generic;



    public partial class ValuesViewerContainer : GridContainer
    {

        private ProfilerMetricsViewerGraph graphControl;

        private int columns = 3;

        //private Dictionary<string, Label[]> valueLabels2 = new Dictionary<string, Label[]>();
        private Dictionary<string, MetricData> valueLabels = new Dictionary<string, MetricData>();

        private int frameCount = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            graphControl = GetTree().Root.FindChild("ProfilerMetricsViewerGraph", true, false) as ProfilerMetricsViewerGraph;

            this.Columns = this.columns;
            ProfilingCollection.NewMetric += NewMetric;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            ProfilingCollection.add("frame");

            ProfilingCollection.ProcessTime(delta);

            foreach (KeyValuePair<string, Metric> entry in ProfilingCollection.values)
            {
                var metric = entry.Value;

                //var labels = valueLabels2[entry.Key];
                //labels[0].Text = "" + metric.currentSum();
                //labels[1].Text = "" + metric.currentInterval();

                var metricData = valueLabels[entry.Key];
                metricData.labels[0].Text = "" + metric.currentSum();
                metricData.labels[1].Text = "" + metric.currentInterval();
                this.graphControl.AddMetricPoint(metricData.graphIndex, new Vector2(frameCount, (float)metric.currentInterval()));
            }

            frameCount++;
        }

        public void ProcessGraph(float delta)
        {
            //timePassed += delta;
            //if (timePassed >= 1)
            //{
            //    seconds++;
            //    Metric metric;
            //    if (this.graphControl != null && ProfilingCollection.values.TryGetValue("frame", out metric))
            //    {
            //        this.graphControl.addPoint(new Vector2(seconds, (float)metric.currentInterval()));
            //    }

            //    timePassed = 0;
            //}

        }

        private void NewMetric(object sender, ProfilingCollection.NewMetricArgs args)
        {
            this.newAttachedLabel(args.name);

            int bufferLength = this.columns - 1;

            Label[] labels = new Label[bufferLength];
            for ( int i = 0; i < bufferLength; i++)
            {
                labels[i] = this.newAttachedLabel("" + 0);
            }

            //valueLabels2[args.name] = labels;

            MetricData data = new MetricData();
            data.labels = labels;
            data.graphIndex = this.graphControl.AddMetric();
            valueLabels[args.name] = data;

        }

        private Label newAttachedLabel(string name)
        {
            Label newLabel = new Label();
            newLabel.Text = name;
            AddChild(newLabel);
            return newLabel;
        }

        private class MetricData
        {
            public Label[] labels;

            public int graphIndex;
        }
    }
