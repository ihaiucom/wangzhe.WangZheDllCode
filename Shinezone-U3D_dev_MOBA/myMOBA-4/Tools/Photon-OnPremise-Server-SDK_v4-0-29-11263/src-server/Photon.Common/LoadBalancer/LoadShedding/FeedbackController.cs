// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackController.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using ExitGames.Logging;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    internal class FeedbackController
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly FeedbackName feedbackName;

        private readonly Dictionary<FeedbackLevel, int> thresholdValues;

        private FeedbackLevel currentFeedbackLevel;

        private int currentInput;

        public FeedbackController(
            FeedbackName feedbackName, Dictionary<FeedbackLevel, int> thresholdValues, int initialInput, FeedbackLevel initalFeedbackLevel)
        {
            this.thresholdValues = thresholdValues;
            this.feedbackName = feedbackName;
            this.currentFeedbackLevel = initalFeedbackLevel;
            this.currentInput = initialInput;
        }

        public FeedbackName FeedbackName
        {
            get
            {
                return this.feedbackName;
            }
        }

        public FeedbackLevel Output
        {
            get
            {
                return this.currentFeedbackLevel;
            }
        }

        public FeedbackLevel GetNextHigherThreshold(FeedbackLevel level, out int result)
        {
            FeedbackLevel next = level;
            while (next != FeedbackLevel.Highest)
            {
                next = FeedbackLevelOrder.GetNextHigher(next);
                if (this.thresholdValues.TryGetValue(next, out result))
                {
                    return next;
                }
            }

            this.thresholdValues.TryGetValue(level, out result);
            return level;
        }

        public FeedbackLevel GetNextLowerThreshold(FeedbackLevel level, out int result)
        {
            FeedbackLevel next = level;
            while (next != FeedbackLevel.Lowest)
            {
                next = FeedbackLevelOrder.GetNextLower(next);
                if (this.thresholdValues.TryGetValue(next, out result))
                {
                    return next;
                }
            }

            this.thresholdValues.TryGetValue(level, out result);
            return level;
        }

        public bool SetInput(int input)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("SetInput: {0} value {1}", this.FeedbackName, input);
            }

            if (input > this.currentInput)
            {
                int threshold;
                FeedbackLevel last = this.currentFeedbackLevel;
                FeedbackLevel next = this.GetNextHigherThreshold(last, out threshold);
                while (next != last)
                {
                    if (input >= threshold)
                    {
                        last = next;
                        next = this.GetNextHigherThreshold(last, out threshold);
                    }
                    else
                    {
                        next = last;
                    }
                }

                this.currentInput = input;
                if (next != this.currentFeedbackLevel)
                {
                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("Transit {0} from {1} to {2} with input {3}", this.FeedbackName, this.currentFeedbackLevel, next, input);
                    }

                    this.currentFeedbackLevel = next;
                    return true;
                }
            }
            else if (input < this.currentInput)
            {
                int threshold;
                FeedbackLevel last = this.currentFeedbackLevel;
                FeedbackLevel next = this.GetNextLowerThreshold(last, out threshold);
                while (next != last)
                {
                    if (input <= threshold)
                    {
                        last = next;
                        next = this.GetNextLowerThreshold(last, out threshold);
                    }
                    else
                    {
                        next = last;
                    }
                }

                this.currentInput = input;
                if (next != this.currentFeedbackLevel)
                {
                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("Transit {0} from {1} to {2} with input {3}", this.FeedbackName, this.currentFeedbackLevel, next, input);
                    }

                    this.currentFeedbackLevel = next;
                    return true;
                }
            }

            return false;
        }
    }
}