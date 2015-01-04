using System.Collections.Generic;
using System.Drawing;
using AiPathFinding.Common;
using AiPathFinding.Properties;

namespace AiPathFinding.Model
{
    /// <summary>
    /// Instance of a special entity like the player or the target
    /// </summary>
    public sealed class Entity
    {
        #region Events

        /// <summary>
        /// Triggered when the entity's node changes.
        /// </summary>
        /// <param name="oldNode">Old node of the entity</param>
        /// <param name="newNode">New node of the entity</param>
        /// <param name="entity">Entity that has been moved</param>
        public delegate void OnNodeChanged(Node oldNode, Node newNode, Entity entity);

        public static event OnNodeChanged NodeChanged;

        #endregion

        #region Instance Fields

        /// <summary>
        /// Node where the entity currently is located
        /// </summary>
        public Node Node
        {
            get { return _node; }
            set
            {
                // only do stuff if something actually changes...
                if (value == _node) return;

                // store old node and update reference (from entity to node)
                var oldNode = _node;
                _node = value;

                // update reverse reference (from node to entity)
                if (oldNode != null)
                    oldNode.EntityOnNode = null;
                if (_node != null)
                    _node.EntityOnNode = this;

                // trigger event
                if (NodeChanged != null)
                    NodeChanged(oldNode, _node, this);
            }
        }

        /// <summary>
        /// Node backing field.
        /// </summary>
        private Node _node;

        /// <summary>
        /// Icon of the entity.
        /// </summary>
        public Icon Icon
        {
            get { return IconMap[Type]; }
        }

        /// <summary>
        /// True if the entity is set somewhere.
        /// </summary>
        public bool IsVisible { get { return Node != null; } }

        /// <summary>
        /// Type of the entity.
        /// </summary>
        public EntityType Type { get; private set; }

        #endregion

        #region Static Fields

        /// <summary>
        /// Player entity.
        /// </summary>
        public static readonly Entity Player = new Entity(EntityType.Player);

        /// <summary>
        /// Target entity.
        /// </summary>
        public static readonly Entity Target = new Entity(EntityType.Target);

        /// <summary>
        /// Array of all entities.
        /// </summary>
        public static Entity[] Entities = {Player, Target};

        /// <summary>
        /// Dictionary that assigns an icon to each entity.
        /// </summary>
        private static readonly Dictionary<EntityType, Icon> IconMap = new Dictionary<EntityType, Icon> { { EntityType.Player, Resources.runner}, { EntityType.Target, Resources.flag } }; 

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new entity of the given type. Instantiated from the static fields.
        /// </summary>
        /// <param name="type">Type of the entity</param>
        private Entity(EntityType type)
        {
            Type = type;
        }

        #endregion
    }
}
