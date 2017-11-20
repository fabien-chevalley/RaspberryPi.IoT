﻿#region References
using Microsoft.Extensions.Configuration;
#endregion

namespace Raspberry.IO.GeneralPurpose.Configuration
{
    /// <summary>
    /// Represents the configuration of the GPIO connection.
    /// </summary>
    public class GpioConnectionConfigurationSection : ConfigurationSection
    {
        #region Constants

        /// <summary>
        /// The default poll interval, in milliseconds.
        /// </summary>
        public const decimal DefaultPollInterval = 50.0m;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        public GpioConnectionConfigurationSection(ConfigurationRoot root, string path) : base(root, path)
        {
        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the driver type.
        /// </summary>
        /// <value>
        /// The name of the driver type.
        /// </value>
        public string DriverTypeName
        {
            get { return (string) this["driver"]; }
            set { this["driver"] = value; }
        }

        /// <summary>
        /// Gets or sets the board connector revision.
        /// </summary>
        /// <value>
        /// The board revision, <c>0</c> for automatic detection, <c>1</c> for model B rev1; <c>2</c> for model B rev2 and model A, <c>3</c> for model B+, A+ and higher.
        /// </value>
        public int BoardConnectorRevision
        {
            get { return int.Parse(this["boardConnectorRevision"]); }
            set { this["boardConnectorRevision"] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the poll interval, in milliseconds.
        /// </summary>
        /// <value>
        /// The poll interval, in millisecond.
        /// </value>
        /// <remarks>
        /// Default value is 50ms.
        /// Values lower than 1ms may be specified on Raspberry Pi using decimal notation.
        /// </remarks>
        public decimal PollInterval
        {
            get { return decimal.Parse(this["pollInterval"]); }
            set { this["pollInterval"] = value.ToString(); }
        }

        #endregion
    }
}