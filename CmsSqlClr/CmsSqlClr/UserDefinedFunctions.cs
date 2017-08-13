using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

public class UserDefinedFunctions
{
    [SqlFunction]
    public static SqlBoolean IsValidEmail(SqlString addr)
    {
        if (addr.IsNull)
            return true;
        var email = addr.Value.Trim();
        if (string.IsNullOrEmpty(email))
            return true;
        var re1 = new Regex(@"^(.*\b(?=\w))\b[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9._-]+\.[A-Z]{2,4}\b\b(?!\w)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        var re2 = new Regex(@"^[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return re1.IsMatch(email) || re2.IsMatch(email);
    }

    [SqlFunction]
    public static SqlString RegexMatch(SqlString subject, SqlString pattern)
    {
        if (subject.IsNull)
            return null;
        var m = Regex.Match(subject.Value ?? "", pattern.Value, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        if (!m.Success)
            return null;
        var g = m.Groups["group"];
        return g.Success ? g.Value : m.Value;
    }

    [SqlFunction]
    public static SqlString AllRegexMatchs(SqlString subject, SqlString pattern)
    {
        if (subject.IsNull)
            return null;
        var list = new List<string>();
        var re = new Regex(pattern.Value);
        var match = re.Match(subject.Value);
        while (match.Success)
        {
            list.Add(match.Value);
            match = match.NextMatch();
        }
        return string.Join("<br>\n", list.ToArray());
    }

    [SqlFunction]
    public static SqlString GetStreet(SqlString address)
    {
        if (address.IsNull)
            return null;
        if (string.IsNullOrWhiteSpace(address.Value))
            return null;
        try
        {
            var s = address.Value.Replace(".", "");
            var la = s.Split(' ').ToList();

            if (AllDigits(la[0]))
                la.RemoveAt(0);
            if (Quadrants.Contains(la[0].ToUpper()))
                la.RemoveAt(0);

            la.Reverse();
            if (AllDigits(la[0]))
                la.RemoveAt(0);
            if (la[0].StartsWith("#"))
                la.RemoveAt(0);
            if (Apartments.Contains(la[0].ToUpper()))
                la.RemoveAt(0);
            if (StreetTypes.Contains(la[0].ToUpper()))
                la.RemoveAt(0);
            la.Reverse();

            var street = string.Join(" ", la);
            return street;
        }
        catch (Exception)
        {
            return null;
        }

    }

    [SqlFunction]
    public static SqlBoolean AllDigits(SqlString s)
    {
        return Regex.IsMatch(s.Value, @"\A[0-9]+\z", RegexOptions.Singleline);
    }
    public static readonly string[] Quadrants =
    {
            "N", "NORTH", "S", "SOUTH", "E", "EAST", "W", "WEST", "NE", "NORTHEAST", "NW", "NORTHWEST", "SE", "SOUTHEAST", "SW", "SOUTHWEST"
        };
    public static readonly string[] Apartments =
    {
            "APARTMENT", "APT", "BUILDING", "BLDG", "DEPARTMENT", "DEPT", "FLOOR", "FL", "HANGAR", "HNGR", "LOT", "LOT", "PIER", "PIER", "ROOM",
            "RM", "SLIP", "SLIP", "SPACE", "SPC", "STOP", "STOP", "SUITE", "STE", "TRAILER", "TRLR", "UNIT", "UNIT", "UPPER", "UPPR",
            "BASEMENT","BSMT", "FRONT","FRNT", "LOBBY","LBBY", "LOWER","LOWR", "OFFICE","OFC", "PENTHOUSE","PH", "REAR", "SIDE"
        };
    public static readonly string[] StreetTypes =
    {
            "ALLEE", "ALLEY", "ALLY", "ALY", "ANEX", "ANNEX", "ANNX", "ANX", "ARC", "ARCADE", "AV", "AVE", "AVEN", "AVENU", "AVENUE", "AVN", "AVNUE",
            "BAYOO", "BCH", "BEACH", "BEND", "BG", "BGS", "BLF", "BLFS", "BLUF", "BLUFF", "BLUFFS", "BLVD", "BND", "BOT", "BOTTM", "BOTTOM", "BOUL",
            "BOULEVARD", "BOULV", "BR", "BRANCH", "BRDGE", "BRG", "BRIDGE", "BRK", "BRKS", "BRNCH", "BROOK", "BROOKS", "BTM", "BURG", "BURGS", "BYP",
            "BYPA", "BYPAS", "BYPASS", "BYPS", "BYU", "CAMP", "CANYN", "CANYON", "CAPE", "CAUSEWAY", "CAUSWAY", "CEN", "CENT", "CENTER", "CENTERS",
            "CENTR", "CENTRE", "CIR", "CIRC", "CIRCL", "CIRCLE", "CIRCLES", "CIRS", "CK", "CLB", "CLF", "CLFS", "CLIFF", "CLIFFS", "CLUB", "CMN", "CMP",
            "CNTER", "CNTR", "CNYN", "COMMON", "COR", "CORNER", "CORNERS", "CORS", "COURSE", "COURT", "COURTS", "COVE", "COVES", "CP", "CPE", "CR", "CRCL",
            "CRCLE", "CRECENT", "CREEK", "CRES", "CRESCENT", "CRESENT", "CREST", "CRK", "CROSSING", "CROSSROAD", "CRSCNT", "CRSE", "CRSENT", "CRSNT",
            "CRSSING", "CRSSNG", "CRST", "CRT", "CSWY", "CT", "CTR", "CTRS", "CTS", "CURV", "CURVE", "CV", "CVS", "CYN", "DALE", "DAM", "DIV", "DIVIDE",
            "DL", "DM", "DR", "DRIV", "DRIVE", "DRIVES", "DRS", "DRV", "DV", "DVD", "EST", "ESTATE", "ESTATES", "ESTS", "EXP", "EXPR", "EXPRESS", "EXPRESSWAY",
            "EXPW", "EXPY", "EXT", "EXTENSION", "EXTENSIONS", "EXTN", "EXTNSN", "EXTS", "FALL", "FALLS", "FERRY", "FIELD", "FIELDS", "FLAT", "FLATS", "FLD",
            "FLDS", "FLS", "FLT", "FLTS", "FORD", "FORDS", "FOREST", "FORESTS", "FORG", "FORGE", "FORGES", "FORK", "FORKS", "FORT", "FRD", "FRDS", "FREEWAY",
            "FREEWY", "FRG", "FRGS", "FRK", "FRKS", "FRRY", "FRST", "FRT", "FRWAY", "FRWY", "FRY", "FT", "FWY", "GARDEN", "GARDENS", "GARDN", "GATEWAY",
            "GATEWY", "GDN", "GDNS", "GLEN", "GLENS", "GLN", "GLNS", "GRDEN", "GRDN", "GRDNS", "GREEN", "GREENS", "GRN", "GRNS", "GROV", "GROVE", "GROVES",
            "GRV", "GRVS", "GTWAY", "GTWY", "HARB", "HARBOR", "HARBORS", "HARBR", "HAVEN", "HAVN", "HBR", "HBRS", "HEIGHT", "HEIGHTS", "HGTS", "HIGHWAY",
            "HIGHWY", "HILL", "HILLS", "HIWAY", "HIWY", "HL", "HLLW", "HLS", "HOLLOW", "HOLLOWS", "HOLW", "HOLWS", "HRBOR", "HT", "HTS", "HVN", "HWAY", "HWY",
            "INLET", "INLT", "IS", "ISLAND", "ISLANDS", "ISLE", "ISLES", "ISLND", "ISLNDS", "ISS", "JCT", "JCTION", "JCTN", "JCTNS", "JCTS", "JUNCTION",
            "JUNCTIONS", "JUNCTN", "JUNCTON", "KEY", "KEYS", "KNL", "KNLS", "KNOL", "KNOLL", "KNOLLS", "KY", "KYS", "LA", "LAKE", "LAKES", "LAND", "LANDING",
            "LANE", "LANES", "LCK", "LCKS", "LDG", "LDGE", "LF", "LGT", "LGTS", "LIGHT", "LIGHTS", "LK", "LKS", "LN", "LNDG", "LNDNG", "LOAF", "LOCK", "LOCKS",
            "LODG", "LODGE", "LOOP", "LOOPS", "MALL", "MANOR", "MANORS", "MDW", "MDWS", "MEADOW", "MEADOWS", "MEDOWS", "MEWS", "MILL", "MILLS", "MISSION",
            "MISSN", "ML", "MLS", "MNR", "MNRS", "MNT", "MNTAIN", "MNTN", "MNTNS", "MOTORWAY", "MOUNT", "MOUNTAIN", "MOUNTAINS", "MOUNTIN", "MSN", "MSSN",
            "MT", "MTIN", "MTN", "MTNS", "MTWY", "NCK", "NECK", "OPAS", "ORCH", "ORCHARD", "ORCHRD", "OVAL", "OVERPASS", "OVL", "PARK", "PARKS", "PARKWAY",
            "PARKWAYS", "PARKWY", "PASS", "PASSAGE", "PATH", "PATHS", "PIKE", "PIKES", "PINE", "PINES", "PK", "PKWAY", "PKWY", "PKWYS", "PKY", "PL", "PLACE",
            "PLAIN", "PLAINES", "PLAINS", "PLAZA", "PLN", "PLNS", "PLZ", "PLZA", "PNE", "PNES", "POINT", "POINTS", "PORT", "PORTS", "PR", "PRAIRIE", "PRARIE",
            "PRK", "PRR", "PRT", "PRTS", "PSGE", "PT", "PTS", "RAD", "RADIAL", "RADIEL", "RADL", "RAMP", "RANCH", "RANCHES", "RAPID", "RAPIDS", "RD", "RDG",
            "RDGE", "RDGS", "RDS", "REST", "RIDGE", "RIDGES", "RIV", "RIVER", "RIVR", "RNCH", "RNCHS", "ROAD", "ROADS", "ROUTE", "ROW", "RPD", "RPDS", "RST",
            "RTE", "RUN", "RVR", "SHL", "SHLS", "SHOAL", "SHOALS", "SHOAR", "SHOARS", "SHORE", "SHORES", "SHR", "SHRS", "SKWY", "SKYWAY", "SMT", "SPG", "SPGS",
            "SPNG", "SPNGS", "SPRING", "SPRINGS", "SPRNG", "SPRNGS", "SPUR", "SPURS", "SQ", "SQR", "SQRE", "SQRS", "SQS", "SQU", "SQUARE", "SQUARES", "ST",
            "STA", "STATION", "STATN", "STN", "STR", "STRA", "STRAV", "STRAVE", "STRAVEN", "STRAVENUE", "STRAVN", "STREAM", "STREET", "STREETS", "STREME",
            "STRM", "STRT", "STRVN", "STRVNUE", "STS", "SUMIT", "SUMITT", "TER", "TERR", "TERRACE", "THROUGHWAY", "TPK", "TPKE", "TR", "TRACE", "TRACES",
            "TRACK", "TRACKS", "TRAFFICWAY", "TRAIL", "TRAILS", "TRAK", "TRCE", "TRFY", "TRK", "TRKS", "TRL", "TRLS", "TRNPK", "TRPK", "TRWY", "TUNEL",
            "TUNL", "TUNLS", "TUNNEL", "TUNNELS", "TUNNL", "TURNPIKE", "TURNPK", "UN", "UNDERPASS", "UNION", "UNIONS", "UNS", "UPAS", "VALLEY", "VALLEYS",
            "VALLY", "VDCT", "VIA", "VIADCT", "VIADUCT", "VIEW", "VIEWS", "VILL", "VILLAG", "VILLAGE", "VILLAGES", "VILLE", "VILLG", "VILLIAGE", "VIS", "VIST",
            "VISTA", "VL", "VLG", "VLGS", "VLLY", "VLY", "VLYS", "VST", "VSTA", "VW", "VWS", "WALK", "WALKS", "WALL", "WAY", "WAYS", "WELL", "WELLS", "WL",
            "WLS", "WY", "XING", "XRD"
        };
}
