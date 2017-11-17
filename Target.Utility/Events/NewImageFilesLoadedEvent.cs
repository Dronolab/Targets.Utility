using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Events;

namespace Target.Utility.Events
{
    public class NewImageFilesLoadedEvent : PubSubEvent<NewImageFilesLoadedEvent>
    {

        #region Constructors

        public NewImageFilesLoadedEvent(List<string> imageFilePath)
        {
            this.ImageFilsePath = imageFilePath;
        }

        public NewImageFilesLoadedEvent()
        {
            
        }

        #endregion

        #region Properties

        public List<string> ImageFilsePath { get; set; }

        #endregion
       
    }
}
