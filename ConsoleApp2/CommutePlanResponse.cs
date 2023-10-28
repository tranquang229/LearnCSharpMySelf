using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    public sealed record CommutePlanResponse
    {
        public IEnumerable<CommuteOption> CommuteOptions { get; init; } = Array.Empty<CommuteOption>();
    }

    public sealed record CommuteOption
    {
        /// <summary>
        /// Gets total travel time if user picks this option to commute.
        /// </summary>
        public TimeSpan TravelTimeDuration { get; init; }

        /// <summary>
        /// Gets specifies the various route legs to be able to commute from the pick up to drop.
        /// </summary>
        public IEnumerable<RouteLeg> RouteLegs { get; init; } = Array.Empty<RouteLeg>();
    }

    public sealed record RouteLeg
    {
        /// <summary>
        /// Gets this will be a unique identifier to specify an option to travel between a pick up and drop.
        /// </summary>
        public string? TripId { get; init; }

        /// <summary>
        /// Gets specifies the start location on the route leg.
        /// </summary>
        public LocationDescriptor StartLocation { get; init; } = new LocationDescriptor();

        /// <summary>
        /// Gets specifies the end location on the route leg.
        /// </summary>
        public LocationDescriptor EndLocation { get; init; } = new LocationDescriptor();

        public TimeSpan EstimatedTravelTime { get; init; }

        public string? RouteId { get; init; }

        public string? RouteName { get; init; }

        /// <summary>
        /// Gets specifies the transit mode type like on demand, pedestrian, public transit etc.
        /// </summary>
        public string ModeType { get; init; } = string.Empty;

        /// <summary>
        /// Gets list of upcoming schedules only if they run in a fixed route.
        /// </summary>
        public IEnumerable<Schedule>? UpcomingSchedules { get; init; }
    }
}

public sealed record Schedule
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

public class LocationDescriptor
{
}