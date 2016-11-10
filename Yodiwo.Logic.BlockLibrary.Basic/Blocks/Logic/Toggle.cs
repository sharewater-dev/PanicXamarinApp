using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Blocks.Logic
{
    [Yodiwo.Logic.UIBasicInfo(true,
                              FriendlyName = "Toggle",
                              StencilCategory = UIStencilCategory.Logic,
                              Description = "Boolean output state will toggle when input trigger is triggered; output initialized to false",
                              FriendlyImageSource = "/Content/img/icons/Generic/toggle.png")]
    public class Toggle : Block
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public InputIO Trigger { get { return _Inputs[0]; } }
        //------------------------------------------------------------------------------------------------------------------------
        public OutputIO OutputA { get { return _Outputs[0]; } set { _Outputs[0] = value; } }
        //------------------------------------------------------------------------------------------------------------------------
        /*** HIDDEN! ***/
        [UIBasicInfo(IsVisible = false,
            FriendlyName = "Trigger type",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "Specify required state that will register the output toggle")]
        public eTriggerType TrigType;

        public enum eTriggerType
        {
            OnTrue,
            OnFalse,
            OnAny
        }
        //------------------------------------------------------------------------------------------------------------------------
        [UIBasicInfo(IsVisible = true,
            FriendlyName = "Initial Output Value",
            InspectorCategory = UIInspectorCategory.Configuration,
            Description = "starting value of output (true or false)")]
        public bool OutputStartValue;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        [UIBasicInfo(IsVisible = true,
                     FriendlyName = "Description",
                     InspectorCategory = UIInspectorCategory.Description,
                     Description = "")]
        public Yodiwo.Logic.Descriptors.Markdown MarkDown =
            new Yodiwo.Logic.Descriptors.Markdown()
            {
                Value = @"### __Toggle__
                
This block toggles its boolean output state when input trigger is triggered.
-- - 

#### __Important!__ ####
The trigger port is treated as all other trigger-type ports in Cyan, i.e. a trigger is registered on a Boolean True value or any non boolean value. 

This way any block output (text, numbers, etc) can be used as a trigger, but a boolean pulse (e.g. a button press) will produce a single trigger.
-- -

*Initial Output Value:*
> Configuration of starting value of output(true in case of ON and false otherwise)."
            };
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Toggle()
            : base(1, 1)
        {
            //setup Input IO types
            Trigger.IOType = typeof(bool);
            Trigger.Name = "Trigger";
            Trigger.Description = "Output state will toggle when this input registers a trigger according to set configuration";
            Trigger.UIHints.IsTrigger = true;

            //init output value
            OutputA.Value = OutputStartValue;
            OutputA.IOType = typeof(bool);
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        protected override IEnumerable<BlockState> Solve()
        {
            if (Trigger.IsTriggered)
            {
                bool trigValue = (bool)Trigger.Value;
                if (TrigType == eTriggerType.OnAny
                 || (trigValue && TrigType == eTriggerType.OnTrue)
                 || (!trigValue && TrigType == eTriggerType.OnFalse))
                {
                    OutputA.Value = !(bool)OutputA.Value;
                }
            }

            //switch block to clean
            yield return BlockState.Clean;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
