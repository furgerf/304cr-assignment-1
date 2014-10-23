using System.Collections.Generic;
using System.Drawing;
using AiPathFinding.Properties;

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

                if (oldNode != null)
                    oldNode.EntityOnNode = null;
                if (_node != null)
                    _node.EntityOnNode = this;

                if (NodeChanged != null)
                    NodeChanged(oldNode, _node, this);
            }
        }

        public Icon Icon
        {
            get { return IconMap[Type]; }
        }

        public bool IsVisible { get { return Node != null; } }

        public EntityType Type { get; private set; }

        public static readonly Entity Player = new Entity(EntityType.Player);

        public static readonly Entity Target = new Entity(EntityType.Target);

        public static Entity[] Entities = {Player, Target};

        private static readonly Dictionary<EntityType, Icon> IconMap = new Dictionary<EntityType, Icon> { { EntityType.Player, Resources.runner}, { EntityType.Target, Resources.flag } }; 

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
