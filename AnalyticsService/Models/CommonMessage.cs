using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsService.Models
{
    public class CommonMessage
    {
        public static string ExceptionMessage = "Something went wrong. Error Message - ";
        public static string EmptyModel = "Pass valid model.";
        public static string EmptyPlaybacks = "Passed playback list is empty";
        public static string AnalyticsRetrived = "Analytics retrived successfully.";
        public static string AnalyticsNotFound = "Analytics not found.";
        public static string AnalyticsDelete = "Analytics deleted successfully.";
        public static string AnalyticsInsert = "Analytics inserted successfully.";
        public static string AnalyticsUpdate = "Analytics updated successfully.";
        public static string TypeRequired = "Valid type required.";
    }
}
