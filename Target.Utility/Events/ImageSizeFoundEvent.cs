using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Target.Utility.Events
{
    public class ImageSizeFoundEvent : PubSubEvent<ImageSizeFoundEvent>
    {

        #region Constructors

        public ImageSizeFoundEvent(string imageSize)
        {
            this.ImageSize = imageSize;
        }

        public ImageSizeFoundEvent()
        {
        }

        #endregion

        #region Properties

        public string ImageSize { get; set; }

        #endregion

    }
}
