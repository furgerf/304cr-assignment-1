using System.Collections.Generic;
using System.Drawing;

namespace AiPathFinding.Model
{
    public class Entity
    {
        #region Events

        public delegate void OnNodeChanged(Node oldNode, Node newNode, Entity entity);

        public static event OnNodeChanged NodeChanged;

        #endregion

        #region Fields

        public Node Node
        {
            get { return _node; }
            set
            {
                if (value == _node) return;
                var oldNode = _node;
                _node = value;
                if (NodeChanged != null)
                    NodeChanged(oldNode, _node, this);
            }
        }

        public Brush Brush
        {
            get { return BrushMap[Type]; }
        }

        public bool IsVisible { get { return Node != null; } }

        public EntityType Type { get; private set; }

        public static readonly Entity Player = new Entity(EntityType.Player);

        public static readonly Entity Target = new Entity(EntityType.Target);

        public static Entity[] Entities = {Player, Target};
        
        private static readonly Dictionary<EntityType, Brush> BrushMap = new Dictionary<EntityType, Brush> { {EntityType.Player, Brushes.Blue}, {EntityType.Target, Brushes.Red}}; 

        private Node _node;

        #endregion

        #region Constructor

        private Entity(EntityType type)
        {
            Type = type;
        }

        #endregion

        #region Main Methods



        #endregion

        #region Event Handling



        #endregion
    }
}
