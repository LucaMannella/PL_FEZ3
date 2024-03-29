﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    using System;
    using System.Drawing;

    /// <summary>
    /// IMotionDetector interface
    /// </summary>
    public interface IMotionDetector
    {
        /// <summary>
        /// Motion level calculation - calculate or not motion level
        /// </summary>
        bool MotionLevelCalculation { set; get; }

        /// <summary>
        /// Motion level - amount of changes in percents
        /// </summary>
        double MotionLevel { get; }

        /// <summary>
        /// Process new frame
        /// </summary>
        void ProcessFrame(ref Bitmap image);

        /// <summary>
        /// Reset detector to initial state
        /// </summary>
        void Reset();
    }
}
