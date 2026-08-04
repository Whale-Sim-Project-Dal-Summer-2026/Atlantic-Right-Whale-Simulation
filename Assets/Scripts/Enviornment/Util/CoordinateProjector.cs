using UnityEngine;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Collections.Generic;

public static class CoordinateProjector
{

    public static Vector2 GeoToUTM(Vector2 coord) {

        int zone = (int)Mathf.Floor((float)((coord.x + 180.0) / 6.0)) + 1;
        bool isNorth = coord.y >= 0.0;
        double centralMeridian = (zone * 6.0) - 183.0;

        CoordinateSystemFactory csFactory = new CoordinateSystemFactory();
        GeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

        List<ProjectionParameter> parameters = new List<ProjectionParameter> {
            new ProjectionParameter("latitude_of_origin", 0.0),
            new ProjectionParameter("central_meridian", centralMeridian),
            new ProjectionParameter("scale_factor", 0.9996),
            new ProjectionParameter("false_easting", 500000.0),
            new ProjectionParameter("false_northing", isNorth ? 0.0 : 10000000.0)
        };

        IProjection projection = csFactory.CreateProjection("Transverse Mercator", "Transverse_Mercator", parameters);

        ProjectedCoordinateSystem utmCrs = csFactory.CreateProjectedCoordinateSystem(
            "UTM", 
            wgs84, 
            projection, 
            LinearUnit.Metre,
            new AxisInfo("Easting", AxisOrientationEnum.East),
            new AxisInfo("Northing", AxisOrientationEnum.North)
        );

        CoordinateTransformationFactory ctFactory = new CoordinateTransformationFactory();
        ICoordinateTransformation transform = ctFactory.CreateFromCoordinateSystems(wgs84, utmCrs);

        double[] result = transform.MathTransform.Transform(new double[] { coord.x, coord.y });

        return new Vector2((float)result[0], (float)result[1]);
    }


    public static Vector2 UTMToGeo(Vector2 UTM, int zone, bool north) {
        double centralMeridian = (zone * 6.0) - 183.0;

        CoordinateSystemFactory csFactory = new CoordinateSystemFactory();
        GeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

        List<ProjectionParameter> parameters = new List<ProjectionParameter> {
            new ProjectionParameter("latitude_of_origin", 0.0),
            new ProjectionParameter("central_meridian", centralMeridian),
            new ProjectionParameter("scale_factor", 0.9996),
            new ProjectionParameter("false_easting", 500000.0),
            new ProjectionParameter("false_northing", north ? 0.0 : 10000000.0)
        };

        IProjection projection = csFactory.CreateProjection("Transverse Mercator", "Transverse_Mercator", parameters);

        ProjectedCoordinateSystem utmCrs = csFactory.CreateProjectedCoordinateSystem(
            "UTM", 
            wgs84, 
            projection, 
            LinearUnit.Metre,
            new AxisInfo("Easting", AxisOrientationEnum.East),
            new AxisInfo("Northing", AxisOrientationEnum.North)
        );

        CoordinateTransformationFactory ctFactory = new CoordinateTransformationFactory();
        
        ICoordinateTransformation transform = ctFactory.CreateFromCoordinateSystems(utmCrs, wgs84);

        double[] result = transform.MathTransform.Transform(new double[] { UTM.x, UTM.y });

        return new Vector2((float)result[0], (float)result[1]);
    }
}