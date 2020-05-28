using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace QSThumbCreator.Utility
{
    /*
     * This utility is sued to get the default styling
     * for a component should we need it.
     */
    public class DefaultTemplateUtility
    {
        public static void SaveDefaultTemplate()
        {
            var checkBoxControl = Application.Current.FindResource(typeof(CheckBox));
            using (XmlTextWriter writer = new XmlTextWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                XamlWriter.Save(checkBoxControl ?? "", writer);
            }
        }
    }
}
