using System.Collections.Generic;
using QSThumbCreator.Models.Qlik;

namespace QSThumbCreator.Models.Thumb
{
    public class QlikThumbModel
    {
        public static string TaskLocalSaveOnly = "TASK_SAVE_ONLY";
        public static string TaskContentDirectorySave = "TASK_MODIFY_APP";

        public List<QlikApp> SelectedQlikApps { get; set; }
        public QlikContentLibrary ContentLibrary { get; set; }
        public string ThumbOutputDirectory { get; set; }
        public string TaskType { get; set; }
    }
}