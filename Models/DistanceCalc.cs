﻿using System;

namespace $safeprojectname$.Models
{
    public static class DistanceCalc
    {
        //public static double GetDistance(double LatEvent, double LngEvent, double LatPassed, double LngPassed, char unit)
        public static double GetDistance(double LatEvent, double LngEvent, double LatPassed, double LngPassed, string unit)
        {
            double theta = LngEvent - LngPassed;
            double dist = Math.Sin(deg2rad(LatEvent)) * Math.Sin(deg2rad(LatPassed)) + Math.Cos(deg2rad(LatEvent)) * Math.Cos(deg2rad(LatPassed)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            //if (unit == 'K')
            if (unit == "K")
            {
                dist = dist * 1.609344;
            }
            //else if (unit == 'N')
            else if (unit == "N")
            {
                dist = dist * 0.8684;
            }
            else if (unit == "Meters")
            {
                dist = dist * 1609.344;
            }
            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        //System.Diagnostics.Debug.WriteLine("Distance in statute miles: " + DistanceCalc.GetDistance(32.9697, -96.80322, 29.46786, -98.53506, "M"));
        //System.Diagnostics.Debug.WriteLine("Distance in kilometers: " + DistanceCalc.GetDistance(32.9697, -96.80322, 29.46786, -98.53506, "K"));
        //System.Diagnostics.Debug.WriteLine("Distance in nautical miles: " + DistanceCalc.GetDistance(32.9697, -96.80322, 29.46786, -98.53506, "N"));
        //System.Diagnostics.Debug.WriteLine("Distance in meters: " + DistanceCalc.GetDistance(32.9697, -96.80322, 29.46786, -98.53506, "Meters"));




    }
}



//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
//:::                                                                         :::
//:::  This routine calculates the distance between two points (given the     :::
//:::  latitude/longitude of those points). It is being used to calculate     :::
//:::  the distance between two locations using GeoDataSource(TM) products    :::
//:::                                                                         :::
//:::  Definitions:                                                           :::
//:::    South latitudes are negative, east longitudes are positive           :::
//:::                                                                         :::
//:::  Passed to function:                                                    :::
//:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
//:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
//:::    unit = the unit you desire for results                               :::
//:::           where: 'M' is statute miles (default)                         :::
//:::                  'K' is kilometers                                      :::
//:::                  'N' is nautical miles                                  :::
//:::                                                                         :::
//:::  Worldwide cities and other features databases with latitude longitude  :::
//:::  are available at http://www.geodatasource.com                          :::
//:::                                                                         :::
//:::  For enquiries, please contact sales@geodatasource.com                  :::
//:::                                                                         :::
//:::  Official Web site: http://www.geodatasource.com                        :::
//:::                                                                         :::
//:::           GeoDataSource.com (C) All Rights Reserved 2015                :::
//:::                                                                         :::
//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
