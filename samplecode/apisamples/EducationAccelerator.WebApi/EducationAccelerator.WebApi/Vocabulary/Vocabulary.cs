/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using CsvHelper;
using System.Collections.Generic;
using System.IO;

// https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452020
namespace EducationAccelerator.Vocabulary
{
    public enum IMSClassType {
        homeroom = 521560000,
        scheduled = 521560001
    }
    public enum Gender {
        male,
        female
    }
    public enum Importance {
        primary = 521560000,
        secondary = 521560001
    }
    public enum OrgType {
        department = 521560000,
        school = 521560001,
        district = 521560002,
        local = 521560003,
        state = 521560004,
        national = 521560005
    }
    public enum RoleType {
        administrator = 521560000,
        aide = 521560001,
        guardian = 521560002,
        parent = 521560003,
        proctor = 521560004,
        relative = 521560005,
        student = 521560006,
        teacher = 521560007
    }
    public enum ScoreStatus {
        exempt = 521560000,
        fully_graded = 521560001,
        not_submitted = 521560002,
        partially_graded = 521560003,
        submitted = 521560004
    }
    public enum SessionType {
        gradingPeriod = 521560000,
        semester = 521560001,
        schoolYear = 521560002,
        term = 521560003
    }
    public enum StatusType {
        active = 521560000,
        tobedeleted = 521560001,
        inactive = 521560001,
    }

    public class Grades
    {
        public static Dictionary<string, string> Members = new Dictionary<string, string>()
        {
            ["521560017"] = "UG",
            ["521560018"] = "Other",
            ["521560000"] = "IT",
            ["521560001"] = "PR",
            ["521560002"] = "PK",
            ["521560003"] = "TK",
            ["521560004"] = "KG",
            ["521560005"] = "01",
            ["521560006"] = "02",
            ["521560007"] = "03",
            ["521560008"] = "04",
            ["521560009"] = "05",
            ["521560010"] = "06",
            ["521560011"] = "07",
            ["521560012"] = "08",
            ["521560013"] = "09",
            ["521560014"] = "10",
            ["521560015"] = "11",
            ["521560016"] = "12",
            ["521560017"] = "13"
        };
    }

    public class SubjectCodes
    {
        public static Dictionary<string, string> SubjectMap;
        public static void Initialize()
        {
            SubjectMap = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader("Vocabulary/sced-v4.csv"))
            {
                var csv = new CsvReader(sr);
                
                while( csv.Read() )
                {
                    var code = csv.GetField<string>(0);
                    var title = csv.GetField<string>(1);
                    SubjectMap.Add(code, title);
                }
            }
        }
    }
}
