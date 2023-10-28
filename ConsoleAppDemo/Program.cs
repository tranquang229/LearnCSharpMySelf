using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleAppDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var objA = new CommutePlanResponse()
            {
                CommuteOptions = new List<CommuteOption>()
                {
                    new CommuteOption()
                    {
                        TravelTimeDuration = TimeSpan.Zero,
                        RouteLegs = new List<RouteLeg>()
                        {
                            new RouteLeg()
                            {
                                TripId = "abc",
                                StartLocation = new LocationDescriptor(),
                                EndLocation = new LocationDescriptor(),
                                EstimatedTravelTime = TimeSpan.Zero,
                                RouteId = "abc",
                                RouteName = "abc",
                                //ModeType = "abc",
                                UpcomingSchedules = new List<Schedule>()
                                {
                                    new Schedule()
                                    {
                                        AvailableBikeRacks = 1,
                                        AvailableSeats = 1,
                                        AvailableWheelChairs = 1,
                                        ScheduleStartTime = new DateTimeOffset(2020,06,01,05,03,03,TimeSpan.Zero),
                                        ScheduleEndTime = new DateTimeOffset(2020,06,01,05,03,03,TimeSpan.Zero),
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var objC = new CommutePlanResponse2()
            {
                CommuteOptions = new List<CommuteOption2>()
                {
                    new CommuteOption2()
                    {
                        TravelTimeDuration = TimeSpan.Zero,
                        RouteLegs = new List<RouteLeg2>()
                        {
                            new RouteLeg2()
                            {
                                TripId = "abc",
                                StartLocation = new LocationDescriptor2(),
                                EndLocation = new LocationDescriptor2(),
                                EstimatedTravelTime = TimeSpan.Zero,
                                RouteId = "abc",
                                RouteName = "abc",
                                UpcomingSchedules = new List<Schedule2>()
                                {
                                    new Schedule2()
                                    {
                                        AvailableBikeRacks = 1,
                                        AvailableSeats = 1,
                                        AvailableWheelChairs = 1,
                                        ScheduleStartTime =new DateTimeOffset(2020,06,01,05,03,03,TimeSpan.Zero),
                                        ScheduleEndTime = new DateTimeOffset(2020,06,01,05,03,03,TimeSpan.Zero),
                                    }
                                }
                            }
                        }
                    }
                }
            };
            //string input1 = JsonConvert.SerializeObject(objA);
            //string input2 = ExtensionDemo<CommutePlanResponse2>.MakeJsonSchema(objC);

            var jdp = new JsonDiffPatch();
            var left = JToken.Parse(JsonConvert.SerializeObject(objA));
            var right = JToken.Parse(JsonConvert.SerializeObject(objC));

            JToken patch = jdp.Diff(left, right);
            var output = jdp.Patch(left, patch);

            Console.WriteLine(output.ToString());
            Console.WriteLine(patch.ToString());

        }
        public static IEnumerable<JProperty> DoCompare(string expectedJSON, string actualJSON)
        {
            // convert JSON to object
            JObject xptJson = JObject.Parse(expectedJSON);
            JObject actualJson = JObject.Parse(actualJSON);

            // read properties
            var xptProps = xptJson.Properties().ToList();
            var actProps = actualJson.Properties().ToList();

            // find missing properties
            var missingProps = xptProps.Where(expected => actProps.Where(actual => actual.Name == expected.Name).Count() == 0);

            return missingProps;
        }

    }
}
