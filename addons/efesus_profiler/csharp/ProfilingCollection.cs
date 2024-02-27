namespace EfesusProfiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;


    static class ProfilingCollection
    {
        static private readonly int MAX_BUFFER = 100;

        static public event EventHandler<NewMetricArgs> NewMetric;

        static public Dictionary<string, Metric> values = new Dictionary<string, Metric>();

        static private double timePassed = 0;
        static private double timeInterval = 1;

        static public void ProcessTime(double delta)
        {

            // refactoringCandidate: this wont work correctly if delta > timeInterval

            timePassed += delta;
            if (timePassed >= timeInterval)
            {
                foreach (KeyValuePair<string, Metric> entry in ProfilingCollection.values)
                {
                    entry.Value.perInterval(timePassed);
                }

                timePassed = 0;
            }
        }

        static public void add(string name)
        {
            Metric val; 
            if ( values.TryGetValue(name, out val) )
            {
                val.tick(1);
            } else
            {
                val = new Metric(MAX_BUFFER);
                val.tick(1);
                values.Add(name, val);
                NewMetricArgs args = new NewMetricArgs();
                args.name = name;
                if (NewMetric != null)
                {
                    NewMetric.Invoke(null, args);
                }
            }
        }

        public partial class NewMetricArgs
        { 
            public string name;
        }
    }

    public partial class Metric
    {
        private int bufferIndex = -1;
        private float bufferSum = 0;
        private float[] buffer;
        private int bufferSize;
        private double bufferAverage = 0;

        private float metricPerInterval = 0;
        private float metricTotal = 0;

        public Metric(int bufferSize)
        {
            this.bufferSize = bufferSize;
            buffer = new float[bufferSize];
        }

        public void tick(float metric)
        {
            this.metricTotal += metric;
            this.metricPerInterval += metric;
        }

        public double currentInterval()
        {
            return bufferAverage;
        }

        public double currentSum()
        {
            return metricTotal;
        }

        public void perInterval(double timePassed)
        {
            this.bufferAverage = metricPerInterval;
            this.metricPerInterval = 0;
        }

        public void perInterval_smoothing(float timePassed)
        {
            if (this.bufferAverage == 0)
            {
                this.bufferAverage = metricPerInterval;
            } else
            {
                this.bufferAverage = (this.bufferAverage * 0.9) + (metricPerInterval * 0.1);
            }
            this.metricPerInterval = 0;
        }

        public void perInterval_buffer(double timePassed)
        {
            if (bufferIndex == -1)
            {
                bufferSum = metricPerInterval * bufferSize;
                for (int i = 0; i < bufferSize; i++)
                {
                    buffer[i] = metricPerInterval;
                }
                bufferIndex++;
            }
            else
            {
                bufferSum -= buffer[bufferIndex];  /* subtract value falling off */
                bufferSum += metricPerInterval;              /* add new value */
            }

            buffer[bufferIndex] = metricPerInterval;   /* save new value so it can be subtracted later */
            if (++bufferIndex == bufferSize)    /* inc buffer index */
                bufferIndex = 0;

            this.metricPerInterval = 0;
            this.bufferAverage = (bufferSum / bufferSize);
        }
    }

