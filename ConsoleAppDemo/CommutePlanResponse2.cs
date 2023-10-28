using System;
using System.Collections.Generic;

namespace ConsoleAppDemo
{
    public sealed record CommutePlanResponse2
    {
        public IEnumerable<CommuteOption2> CommuteOptions { get; init; } = Array.Empty<CommuteOption2>();
    }

    public sealed record CommuteOption2
    {
        /// <summary>
        /// Gets total travel time if user picks this option to commute.
        /// </summary>
        public TimeSpan TravelTimeDuration { get; init; }

        /// <summary>
        /// Gets specifies the various route legs to be able to commute from the pick up to drop.
        /// </summary>
        public IEnumerable<RouteLeg2> RouteLegs { get; init; } = Array.Empty<RouteLeg2>();
    }

    public sealed record RouteLeg2
    {
        /// <summary>
        /// Gets this will be a unique identifier to specify an option to travel between a pick up and drop.
        /// </summary>
        public string? TripId { get; init; }

        /// <summary>
        /// Gets specifies the start location on the route leg.
        /// </summary>
        public LocationDescriptor2 StartLocation { get; init; } = new LocationDescriptor2();

        /// <summary>
        /// Gets specifies the end location on the route leg.
        /// </summary>
        public LocationDescriptor2 EndLocation { get; init; } = new LocationDescriptor2();

        public TimeSpan EstimatedTravelTime { get; init; }

        public string? RouteId { get; init; }

        public string? RouteName { get; init; }

        /// <summary>
        /// Gets specifies the transit mode type like on demand, pedestrian, public transit etc.
        /// </summary>
        //public string ModeType { get; init; } = string.Empty;

        /// <summary>
        /// Gets list of upcoming schedules only if they run in a fixed route.
        /// </summary>
        public IEnumerable<Schedule2>? UpcomingSchedules { get; init; }
    }
}

public sealed record Schedule2
{
    /// <summary>
    /// Gets schedule start time.
    /// </summary>
    public DateTimeOffset ScheduleStartTime { get; init; }

    /// <summary>
    /// Gets schedule drop time.
    /// </summary>
    public DateTimeOffset ScheduleEndTime { get; init; }

    /// <summary>
    /// Gets available seats on that specific pick and drop time of the route.
    /// </summary>
    public int AvailableSeats { get; init; }
    public int AvailableBikeRacks { get; init; }
    public int AvailableWheelChairs { get; init; }
}

public class LocationDescriptor2
{
}