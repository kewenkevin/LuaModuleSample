

namespace ND.UI
{
    public class LinkerData
    {
        private string _label;

        private int _valueTypeId;

        // private int _gameObjectId;

        // private int _binderId;

        // private int _linkerId;

        public string Label { get => _label; set => _label = value; }
        public int ValueTypeId { get => _valueTypeId; set => _valueTypeId = value; }

        // public int GameObjectId { get => _gameObjectId; set => _gameObjectId = value; }
        // public int BinderId { get => _binderId; set => _binderId = value; }
        // public int LinkerId { get => _linkerId; set => _linkerId = value; }

        public LinkerData()
        {

        }

        public LinkerData(UIExpansion owner, LinkerConfig config)
        {
            Init(owner, config);
        }

        public bool Init(UIExpansion owner, LinkerConfig config)
        {
            _label = owner.StoredStrings[config.LabelIndex];
            // _gameObjectId = config.StoredGameObjectIndex;
            // _binderId = owner.StoredInts[config.BinderTypeIndex];
            // _linkerId = owner.StoredInts[config.LinkerTypeIndex];
            _valueTypeId = owner.StoredInts[config.ValueTypeIndex];
            return true;
        }
    }
}