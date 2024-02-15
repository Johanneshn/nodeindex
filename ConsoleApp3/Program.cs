var timeRange1 = new TimeRange() { Start = new DateTime(2024, 1, 24), End = new DateTime(2024, 1, 25) };
var timeRange2 = new TimeRange() { Start = new DateTime(2024, 1, 25), End = new DateTime(2024, 1, 26) };
var timeRanges = new List<TimeRange> { timeRange1 };

var dataPoints = new List<DataPoint>()
{
    new DataPoint { Name = "A", Value = 10, NodeIndex = 1, Time = new DateTime(2023, 1, 24, 9, 0, 0) },
    new DataPoint { Name = "A", Value = 15, NodeIndex = 1, Time = new DateTime(2024, 1, 24, 8, 0, 0) },
    new DataPoint { Name = "A", Value = 20, NodeIndex = 1, Time = new DateTime(2024, 1, 24, 9, 0, 0) },
    new DataPoint { Name = "B", Value = 30, NodeIndex = 2, Time = new DateTime(2024, 1, 24, 10, 0, 0) },
    // new DataPoint { Name = "B", Value = 3000, NodeIndex = 2, Time = new DateTime(2024, 1, 24, 10, 1, 0) },
    // new DataPoint { Name = "C", Value = 4, NodeIndex = 2, Time = new DateTime(2024, 1, 25, 10, 0, 0) }
};

foreach (var dataPoint in dataPoints)
{
    foreach (var timeRange in timeRanges)
    {
        if (timeRange.Start <= dataPoint.Time && dataPoint.Time <= timeRange.End)
        {
            timeRange.AddDataPoint(dataPoint);
        }
        else if (timeRange.Start >= dataPoint.Time)
        {
            timeRange.TryAddPointBeforeStart(dataPoint);
        }
    }
}


var x = timeRange1.CalculateDeltaFor("A");

Console.ReadLine();

public class DataPointTypeCollection
{
    public int[] AvailableNodes => DataPointsByNodeIndex.Keys.ToArray();
    private Dictionary<int, List<DataPoint>> DataPointsByNodeIndex { get; } = new();
    private Dictionary<int, DataPoint> UniqueDataPointOutsideTimeRange { get; } = new();

    public void Add(DataPoint point)
    {
        if (DataPointsByNodeIndex.ContainsKey(point.NodeIndex) is false)
        {
            DataPointsByNodeIndex[point.NodeIndex] = [];
        }

        DataPointsByNodeIndex[point.NodeIndex].Add(point); 
    }

    public void AddBeforeRange(DataPoint point)
    {
        UniqueDataPointOutsideTimeRange[point.NodeIndex] = point;
    }
    
    public DataPoint? GetLastDataPoint(int node)
    {
        return DataPointsByNodeIndex.TryGetValue(node, out var value) ? value.LastOrDefault() : null;
    }
    
    public DataPoint? GetFirstDataPoint(int node)
    {
        return DataPointsByNodeIndex.TryGetValue(node, out var value) ? value.FirstOrDefault() : null;
    }
    
    public DataPoint? GetDataPointBeforeRange(int node)
    {
        return UniqueDataPointOutsideTimeRange.GetValueOrDefault(node);

    }
}

public class TimeRange
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public Dictionary<string, DataPointTypeCollection> DataPointTypeCollections { get; } = new();

    public void AddDataPoint(DataPoint point)
    {
        if (DataPointTypeCollections.ContainsKey(point.Name) is false)
        {
            DataPointTypeCollections[point.Name] = new DataPointTypeCollection();
        }

        DataPointTypeCollections[point.Name].Add(point);
    }
    
    public void TryAddPointBeforeStart(DataPoint point)
    {
        if (DataPointTypeCollections.ContainsKey(point.Name) is false)
        {
            DataPointTypeCollections[point.Name] = new DataPointTypeCollection();
        } 
        DataPointTypeCollections[point.Name].AddBeforeRange(point);
    }
    
    public List<double> CalculateDeltaFor(string name)
    {
        var result = new List<double>();
        
        var points = DataPointTypeCollections[name];
       
        foreach (var node in points.AvailableNodes)
        {
            var last = points.GetLastDataPoint(node);
         
            if (last is null)
            {
                result.Add(0.0);
                continue;
            }


            var first = points.GetDataPointBeforeRange(node) ?? points.GetFirstDataPoint(node);
            if (first is null)
            {
                result.Add(0.0);
                continue;
            }
            
            result.Add(last.Value - first.Value);
        }

        return result;
    }
}


public class DataPoint
{
    public string Name { get; set; }
    public double Value { get; set; }
    public int NodeIndex { get; set; }
    public DateTime Time { get; set; }
}