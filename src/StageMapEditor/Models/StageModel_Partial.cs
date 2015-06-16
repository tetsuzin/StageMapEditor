

using System;
using System.Collections.Generic;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Models
{
	public partial class StageModel
	{
        Int32 _World;
        public Int32 World
        {
            get
            { return _World; }
            set
            {
                if (_World == value)
                    return;
                _World = value;
                RaisePropertyChanged("World");
            }
        }

        Int32 _Stage;
        public Int32 Stage
        {
            get
            { return _Stage; }
            set
            {
                if (_Stage == value)
                    return;
                _Stage = value;
                RaisePropertyChanged("Stage");
            }
        }

        String _StageName;
        public String StageName
        {
            get
            { return _StageName; }
            set
            {
                if (_StageName == value)
                    return;
                _StageName = value;
                RaisePropertyChanged("StageName");
            }
        }

        String _StageDescription;
        public String StageDescription
        {
            get
            { return _StageDescription; }
            set
            {
                if (_StageDescription == value)
                    return;
                _StageDescription = value;
                RaisePropertyChanged("StageDescription");
            }
        }


        public void UpdateFromInput(EditStageViewModel editStageViewModel)
        {
			World = Int32.Parse(editStageViewModel.InputWorld);
			Stage = Int32.Parse(editStageViewModel.InputStage);
			StageName = editStageViewModel.InputStageName;
			StageDescription = editStageViewModel.InputStageDescription;
         
        }

        private void SetProperties(EditStageViewModel inputViewModel)
        {
			this.World = System.Int32.Parse(inputViewModel.InputWorld);
			this.Stage = System.Int32.Parse(inputViewModel.InputStage);
			this.StageName = inputViewModel.InputStageName;
			this.StageDescription = inputViewModel.InputStageDescription;
        }

        private void SetProperties(StageModelPack stageModelPack)
        {
			this.World = stageModelPack.World;
			this.Stage = stageModelPack.Stage;
			this.StageName = stageModelPack.StageName;
			this.StageDescription = stageModelPack.StageDescription;
        }
	}
}
