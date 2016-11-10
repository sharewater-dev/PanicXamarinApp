using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Endpoints.In
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "DateTime",
                              StencilCategory = UIStencilCategory.Input,
                              Description = "Provide Date and Time information",
                              FriendlyImageSource = "/Content/img/icons/Generic/datetime.png")]
    public class DateTimeBlock : EndpointIn
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __DateTime__ 

This block provides several Date and Time information.
-- -
DateTime block has eight descrete outputs providing different information.
- Date/Time
- Time
- Date
- Month
- Day of Month
- Day of Week
- Hours
- Minutes
- Seconds"
            };
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Time Zone",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "")]
        public OptionsList SelectedTimeZone = new OptionsList()
        {
            Options = TimeZones.Select(t => t.DisplayName).ToList(),
            selection = 37,
        };
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Gradient Values",
                     InspectorCategory = UIInspectorCategory.Configuration,
                     Description = "When enabling this the outputs will be floating points instead of integers. (eg. for time 8:30 the 'Hour' output will be 8.5)")]
        public bool gradientValues = false;
        //------------------------------------------------------------------------------------------------------------------------
        static readonly TimeZoneInfo[] TimeZones = TimeZoneInfo.GetSystemTimeZones().ToArray();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public DateTimeBlock()
            : base(0, 9)
        {
            //mark as a volatile block
            IsVolatile = true;

            //setup Output IO
            _Outputs[0].Name = "Date/Time";
            _Outputs[1].Name = "Time";
            _Outputs[2].Name = "Date";
            _Outputs[3].Name = "Month";
            _Outputs[4].Name = "Day of Month";
            _Outputs[5].Name = "Day in Week";
            _Outputs[6].Name = "Hours";
            _Outputs[7].Name = "Minutes";
            _Outputs[8].Name = "Seconds";

            _Outputs[0].IOType = typeof(string);
            _Outputs[1].IOType = typeof(string);
            _Outputs[2].IOType = typeof(string);
            _Outputs[3].IOType = typeof(double);
            _Outputs[4].IOType = typeof(double);
            _Outputs[5].IOType = typeof(double);
            _Outputs[6].IOType = typeof(double);
            _Outputs[7].IOType = typeof(double);
            _Outputs[8].IOType = typeof(double);

            _Outputs[0].Value = default(DateTime);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            try
            {
                //get timezone adjusted now
                var utcnow = System.DateTime.UtcNow;
                var timezone = TimeZones[SelectedTimeZone.selection];
#if NETFX
                var now = TimeZoneInfo.ConvertTimeFromUtc(utcnow, timezone);
#elif UNIVERSAL
                var now = TimeZoneInfo.ConvertTime(utcnow, timezone);
#endif

                //apply solution to Outputs
                _Outputs[0].Value = now;
                _Outputs[1].Value = now.TimeOfDay;
                _Outputs[2].Value = now.Date;

                if (gradientValues)
                {
                    var second = now.Second + (now.Millisecond / 1000d);
                    var minute = now.Minute + (second / 60d);
                    var hour = now.Hour + (minute / 60d);
                    var day = now.Day + (hour / 24d);
                    var month = now.Month + (day / (double)DateTime.DaysInMonth(now.Year, now.Month));

                    _Outputs[3].Value = month;
                    _Outputs[4].Value = day;
                    _Outputs[5].Value = (double)now.DayOfWeek + (hour / 24d);
                    _Outputs[6].Value = hour;
                    _Outputs[7].Value = minute;
                    _Outputs[8].Value = second;
                }
                else
                {
                    _Outputs[3].Value = now.Month;
                    _Outputs[4].Value = now.Day;
                    _Outputs[5].Value = now.DayOfWeek;
                    _Outputs[6].Value = now.Hour;
                    _Outputs[7].Value = now.Minute;
                    _Outputs[8].Value = now.Second;
                }
            }
            catch (Exception ex) { }

            //switch block to clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
