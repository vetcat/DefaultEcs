﻿using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs.Technical;
using DefaultEcs.Technical.Message;

namespace DefaultEcs
{
    /// <summary>
    /// Represent an helper object to create rules to retrieve a specific subset of <see cref="Entity"/>.
    /// </summary>
    public sealed class EntityRuleBuilder
    {
        #region Types

        internal enum EitherType
        {
            With,
            Without,
            WhenAdded,
            WhenChanged,
            WhenRemoved
        }

        /// <summary>
        /// Represents an helper object to create an either group rule to retrieve a specific subset of <see cref="Entity"/>.
        /// </summary>
        public sealed class EitherBuilder
        {
            #region Fields

            private readonly EntityRuleBuilder _builder;
            private readonly EitherType _type;

            private ComponentEnum _eitherFilter;

            #endregion

            #region Initialisation

            internal EitherBuilder(EntityRuleBuilder builder, EitherType type)
            {
                _builder = builder;
                _type = type;
            }

            #endregion

            #region Methods

            private EitherBuilder OrWith<T>()
            {
                _builder._addCreated = false;

                ComponentFlag flag = ComponentManager<T>.Flag;
                if (!_eitherFilter[flag])
                {
                    _eitherFilter[flag] = true;
                    if (!_builder._withEitherFilter[flag])
                    {
                        _builder._withEitherFilter[flag] = true;
                        _builder._nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentAddedMessage<T>>(s.CheckedAdd));
                        _builder._nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentEnabledMessage<T>>(s.CheckedAdd));
                        _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentRemovedMessage<T>>(s.CheckedRemove));
                        _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentDisabledMessage<T>>(s.CheckedRemove));
                    }
                }

                return this;
            }

            private EitherBuilder OrWithout<T>()
            {
                ComponentFlag flag = ComponentManager<T>.Flag;
                if (!_eitherFilter[flag])
                {
                    _eitherFilter[flag] = true;
                    if (!_builder._withoutEitherFilter[flag])
                    {
                        _builder._withoutEitherFilter[flag] = true;
                        _builder._nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentRemovedMessage<T>>(s.CheckedAdd));
                        _builder._nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentDisabledMessage<T>>(s.CheckedAdd));
                        _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentAddedMessage<T>>(s.CheckedRemove));
                        _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentEnabledMessage<T>>(s.CheckedRemove));
                    }
                }

                return this;
            }

            private EitherBuilder OrWhenAdded<T>()
            {
                if (!_builder._whenAddedFilter[ComponentManager<T>.Flag])
                {
                    _builder._whenAddedFilter[ComponentManager<T>.Flag] = true;
                    _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentAddedMessage<T>>(s.CheckedAdd));
                    _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentEnabledMessage<T>>(s.CheckedAdd));
                }

                return OrWith<T>();
            }

            private EitherBuilder OrWhenChanged<T>()
            {
                if (!_builder._whenChangedFilter[ComponentManager<T>.Flag])
                {
                    _builder._whenChangedFilter[ComponentManager<T>.Flag] = true;
                    _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentChangedMessage<T>>(s.CheckedAdd));
                }

                return OrWith<T>();
            }

            private EitherBuilder OrWhenRemoved<T>()
            {
                if (!_builder._whenRemovedFilter[ComponentManager<T>.Flag])
                {
                    _builder._whenRemovedFilter[ComponentManager<T>.Flag] = true;
                    _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentRemovedMessage<T>>(s.CheckedAdd));
                    _builder._subscriptions.Add((s, w) => w.Subscribe<ComponentDisabledMessage<T>>(s.CheckedAdd));
                }

                return OrWithout<T>();
            }

            internal EntityRuleBuilder Commit()
            {
                if (_type == EitherType.Without || _type == EitherType.WhenRemoved)
                {
                    _builder.AddWithoutEitherFilter(_eitherFilter);
                }
                else
                {
                    _builder.AddWithEitherFilter(_eitherFilter);
                }

                return _builder;
            }

            /// <summary>
            /// Add the type <typeparamref name="T"/> to current either group.
            /// </summary>
            /// <typeparam name="T">The type of component to add to the either group.</typeparam>
            /// <returns>The current <see cref="EitherBuilder"/>.</returns>
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            public EitherBuilder Or<T>() => _type switch
            {
                EitherType.With => OrWith<T>(),
                EitherType.Without => OrWithout<T>(),
                EitherType.WhenAdded => OrWhenAdded<T>(),
                EitherType.WhenChanged => OrWhenChanged<T>(),
                EitherType.WhenRemoved => OrWhenRemoved<T>()
            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

            /// <summary>
            /// Makes a rule to observe <see cref="Entity"/> with a component of type <typeparamref name="T"/>.
            /// </summary>
            /// <typeparam name="T">The type of component.</typeparam>
            /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
            public EntityRuleBuilder With<T>() => Commit().With<T>();

            /// <summary>
            /// Makes a rule to ignore <see cref="Entity"/> with a component of type <typeparamref name="T"/>.
            /// </summary>
            /// <typeparam name="T">The type of component.</typeparam>
            /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
            public EntityRuleBuilder Without<T>() => Commit().Without<T>();

            /// <summary>
            /// Makes a rule to observe <see cref="Entity"/> when a component of type <typeparamref name="T"/> is added.
            /// </summary>
            /// <typeparam name="T">The type of component.</typeparam>
            /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
            public EntityRuleBuilder WhenAdded<T>() => Commit().WhenAdded<T>();

            /// <summary>
            /// Makes a rule to observe <see cref="Entity"/> when a component of type <typeparamref name="T"/> is changed.
            /// </summary>
            /// <typeparam name="T">The type of component.</typeparam>
            /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
            public EntityRuleBuilder WhenChanged<T>() => Commit().WhenChanged<T>();

            /// <summary>
            /// Makes a rule to observe <see cref="Entity"/> when a component of type <typeparamref name="T"/> is removed.
            /// </summary>
            /// <typeparam name="T">The type of component.</typeparam>
            /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
            public EntityRuleBuilder WhenRemoved<T>() => Commit().WhenRemoved<T>();

            /// <summary>
            /// Makes a rule to obsverve <see cref="Entity"/> with at least one component of the either group.
            /// </summary>
            /// <typeparam name="T">The type of component to add to the either group.</typeparam>
            /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
            public EitherBuilder WithEither<T>() => Commit().WithEither<T>();

            /// <summary>
            /// Makes a rule to obsverve <see cref="Entity"/> without at least one component of the either group.
            /// </summary>
            /// <typeparam name="T">The type of component to add to the either group.</typeparam>
            /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
            public EitherBuilder WithoutEither<T>() => Commit().WithoutEither<T>();

            /// <summary>
            /// Makes a rule to observe <see cref="Entity"/> when one component of the either group is added.
            /// </summary>
            /// <typeparam name="T">The type of component to add to the either group.</typeparam>
            /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
            public EitherBuilder WhenAddedEither<T>() => Commit().WhenAddedEither<T>();

            /// <summary>
            /// Makes a rule to observe <see cref="Entity"/> when one component of the either group is changed.
            /// </summary>
            /// <typeparam name="T">The type of component to add to the either group.</typeparam>
            /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
            public EitherBuilder WhenChangedEither<T>() => Commit().WhenChangedEither<T>();

            /// <summary>
            /// Makes a rule to observe <see cref="Entity"/> when one component of the either group is removed.
            /// </summary>
            /// <typeparam name="T">The type of component to add to the either group.</typeparam>
            /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
            public EitherBuilder WhenRemovedEither<T>() => Commit().WhenRemovedEither<T>();

            /// <summary>
            /// Returns a <see cref="Predicate{T}"/> representing the specified rules.
            /// </summary>
            /// <returns>The <see cref="Predicate{T}"/>.</returns>
            public Predicate<Entity> AsPredicate() => Commit().AsPredicate();

            /// <summary>
            /// Returns an <see cref="EntitySet"/> with the specified rules.
            /// </summary>
            /// <returns>The <see cref="EntitySet"/>.</returns>
            public EntitySet AsSet() => Commit().AsSet();

            /// <summary>
            /// Returns an <see cref="EntityMap{TKey}"/> with the specified rules.
            /// </summary>
            /// <typeparam name="TKey">The component type to use as key.</typeparam>
            /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}.Default"/> for the type of the key.</param>
            /// <returns>The <see cref="EntityMap{TKey}"/>.</returns>
            public EntityMap<TKey> AsMap<TKey>(IEqualityComparer<TKey> comparer) => Commit().AsMap(comparer);

            /// <summary>
            /// Returns an <see cref="EntityMap{TKey}"/> with the specified rules.
            /// </summary>
            /// <typeparam name="TKey">The component type to use as key.</typeparam>
            /// <returns>The <see cref="EntityMap{TKey}"/>.</returns>
            public EntityMap<TKey> AsMap<TKey>() => Commit().AsMap<TKey>();

            /// <summary>
            /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
            /// </summary>
            /// <typeparam name="TKey">The component type to use as key.</typeparam>
            /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
            /// <param name="factory">The factory used to create the <see cref="Entity"/> collection <typeparamref name="TEntities"/>.</param>
            /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}.Default"/> for the type of the key.</param>
            /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
            public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>(Func<TEntities> factory, IEqualityComparer<TKey> comparer) where TEntities : class, ICollection<Entity> => Commit().AsMap(factory, comparer);

            /// <summary>
            /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
            /// </summary>
            /// <typeparam name="TKey">The component type to use as key.</typeparam>
            /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
            /// <param name="factory">The factory used to create the <see cref="Entity"/> collection <typeparamref name="TEntities"/>.</param>
            /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
            public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>(Func<TEntities> factory) where TEntities : class, ICollection<Entity> => Commit().AsMap<TKey, TEntities>(factory);

            /// <summary>
            /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
            /// </summary>
            /// <typeparam name="TKey">The component type to use as key.</typeparam>
            /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
            /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}.Default"/> for the type of the key.</param>
            /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
            public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>(IEqualityComparer<TKey> comparer) where TEntities : class, ICollection<Entity>, new() => Commit().AsMap<TKey, TEntities>(comparer);

            /// <summary>
            /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
            /// </summary>
            /// <typeparam name="TKey">The component type to use as key.</typeparam>
            /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
            /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
            public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>() where TEntities : class, ICollection<Entity>, new() => Commit().AsMap<TKey, TEntities>();

            #endregion
        }

        #endregion

        #region Fields

        private readonly World _world;
        private readonly List<Func<EntityContainerWatcher, World, IDisposable>> _subscriptions;
        private readonly List<Func<EntityContainerWatcher, World, IDisposable>> _nonReactSubscriptions;

        private bool _addCreated;
        private ComponentEnum _withFilter;
        private ComponentEnum _withEitherFilter;
        private ComponentEnum _withoutFilter;
        private ComponentEnum _withoutEitherFilter;
        private ComponentEnum _whenAddedFilter;
        private ComponentEnum _whenChangedFilter;
        private ComponentEnum _whenRemovedFilter;
        private List<ComponentEnum> _withEitherFilters;
        private List<ComponentEnum> _withoutEitherFilters;

        #endregion

        #region Initialisation

        internal EntityRuleBuilder(World world, bool withEnabledEntities)
        {
            _world = world;

            _subscriptions = new List<Func<EntityContainerWatcher, World, IDisposable>>();
            _nonReactSubscriptions = new List<Func<EntityContainerWatcher, World, IDisposable>>();

            _addCreated = withEnabledEntities;
            _subscriptions.Add((s, w) => w.Subscribe<EntityDisposingMessage>(s.Remove));
            _withFilter[World.IsAliveFlag] = true;
            if (withEnabledEntities)
            {
                _withFilter[World.IsEnabledFlag] = true;
                _subscriptions.Add((s, w) => w.Subscribe<EntityDisabledMessage>(s.Remove));
                _nonReactSubscriptions.Add((s, w) => w.Subscribe<EntityEnabledMessage>(s.CheckedAdd));
            }
            else
            {
                _withoutFilter[World.IsEnabledFlag] = true;
                _subscriptions.Add((s, w) => w.Subscribe<EntityEnabledMessage>(s.Remove));
                _nonReactSubscriptions.Add((s, w) => w.Subscribe<EntityDisabledMessage>(s.CheckedAdd));
            }
        }

        #endregion

        #region Methods

        private Predicate<ComponentEnum> GetFilter() => EntityRuleFilterFactory.GetFilter(_withFilter, _withoutFilter, _withEitherFilters, _withoutEitherFilters);

        private bool GetSubscriptions(out List<Func<EntityContainerWatcher, World, IDisposable>> subscriptions)
        {
            subscriptions = _subscriptions.ToList();
            bool hasWhenFilter = !_whenAddedFilter.IsNull || !_whenChangedFilter.IsNull || !_whenRemovedFilter.IsNull;
            if (!hasWhenFilter)
            {
                subscriptions.AddRange(_nonReactSubscriptions);
            }

            if (_addCreated && !hasWhenFilter)
            {
                subscriptions.Add((s, w) => w.Subscribe<EntityCreatedMessage>(s.Add));
            }

            return hasWhenFilter;
        }

        private void AddWithEitherFilter(ComponentEnum filter)
        {
            if (!_withEitherFilters?.Select(f => f.ToString()).Contains(filter.ToString()) ?? true)
            {
                (_withEitherFilters ?? (_withEitherFilters = new List<ComponentEnum>())).Add(filter);
            }
        }

        private void AddWithoutEitherFilter(ComponentEnum filter)
        {
            if (!_withoutEitherFilters?.Select(f => f.ToString()).Contains(filter.ToString()) ?? true)
            {
                (_withoutEitherFilters ?? (_withoutEitherFilters = new List<ComponentEnum>())).Add(filter);
            }
        }

        /// <summary>
        /// Makes a rule to observe <see cref="Entity"/> with a component of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
        public EntityRuleBuilder With<T>()
        {
            _addCreated = false;

            if (!_withFilter[ComponentManager<T>.Flag])
            {
                _withFilter[ComponentManager<T>.Flag] = true;
                _nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentAddedMessage<T>>(s.CheckedAdd));
                _nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentEnabledMessage<T>>(s.CheckedAdd));
                _subscriptions.Add((s, w) => w.Subscribe<ComponentRemovedMessage<T>>(s.Remove));
                _subscriptions.Add((s, w) => w.Subscribe<ComponentDisabledMessage<T>>(s.Remove));
            }

            return this;
        }

        /// <summary>
        /// Makes a rule to ignore <see cref="Entity"/> with a component of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
        public EntityRuleBuilder Without<T>()
        {
            if (!_withoutFilter[ComponentManager<T>.Flag])
            {
                _withoutFilter[ComponentManager<T>.Flag] = true;
                _nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentRemovedMessage<T>>(s.CheckedAdd));
                _nonReactSubscriptions.Add((s, w) => w.Subscribe<ComponentDisabledMessage<T>>(s.CheckedAdd));
                _subscriptions.Add((s, w) => w.Subscribe<ComponentAddedMessage<T>>(s.Remove));
                _subscriptions.Add((s, w) => w.Subscribe<ComponentEnabledMessage<T>>(s.Remove));
            }

            return this;
        }

        /// <summary>
        /// Makes a rule to observe <see cref="Entity"/> when a component of type <typeparamref name="T"/> is added.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
        public EntityRuleBuilder WhenAdded<T>()
        {
            if (!_whenAddedFilter[ComponentManager<T>.Flag])
            {
                _whenAddedFilter[ComponentManager<T>.Flag] = true;
                _subscriptions.Add((s, w) => w.Subscribe<ComponentAddedMessage<T>>(s.CheckedAdd));
                _subscriptions.Add((s, w) => w.Subscribe<ComponentEnabledMessage<T>>(s.CheckedAdd));
            }

            return With<T>();
        }

        /// <summary>
        /// Makes a rule to observe <see cref="Entity"/> when a component of type <typeparamref name="T"/> is changed.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
        public EntityRuleBuilder WhenChanged<T>()
        {
            if (!_whenChangedFilter[ComponentManager<T>.Flag])
            {
                _whenChangedFilter[ComponentManager<T>.Flag] = true;
                _subscriptions.Add((s, w) => w.Subscribe<ComponentChangedMessage<T>>(s.CheckedAdd));
            }

            return With<T>();
        }

        /// <summary>
        /// Makes a rule to observe <see cref="Entity"/> when a component of type <typeparamref name="T"/> is removed.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>The current <see cref="EntityRuleBuilder"/>.</returns>
        public EntityRuleBuilder WhenRemoved<T>()
        {
            if (!_whenRemovedFilter[ComponentManager<T>.Flag])
            {
                _whenRemovedFilter[ComponentManager<T>.Flag] = true;
                _subscriptions.Add((s, w) => w.Subscribe<ComponentRemovedMessage<T>>(s.CheckedAdd));
                _subscriptions.Add((s, w) => w.Subscribe<ComponentDisabledMessage<T>>(s.CheckedAdd));
            }

            return Without<T>();
        }

        /// <summary>
        /// Makes a rule to obsverve <see cref="Entity"/> with at least one component of the either group.
        /// </summary>
        /// <typeparam name="T">The type of component to add to the either group.</typeparam>
        /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
        public EitherBuilder WithEither<T>() => new EitherBuilder(this, EitherType.With).Or<T>();

        /// <summary>
        /// Makes a rule to obsverve <see cref="Entity"/> without at least one component of the either group.
        /// </summary>
        /// <typeparam name="T">The type of component to add to the either group.</typeparam>
        /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
        public EitherBuilder WithoutEither<T>() => new EitherBuilder(this, EitherType.Without).Or<T>();

        /// <summary>
        /// Makes a rule to observe <see cref="Entity"/> when one component of the either group is added.
        /// </summary>
        /// <typeparam name="T">The type of component to add to the either group.</typeparam>
        /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
        public EitherBuilder WhenAddedEither<T>() => new EitherBuilder(this, EitherType.WhenAdded).Or<T>();

        /// <summary>
        /// Makes a rule to observe <see cref="Entity"/> when one component of the either group is changed.
        /// </summary>
        /// <typeparam name="T">The type of component to add to the either group.</typeparam>
        /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
        public EitherBuilder WhenChangedEither<T>() => new EitherBuilder(this, EitherType.WhenChanged).Or<T>();

        /// <summary>
        /// Makes a rule to observe <see cref="Entity"/> when one component of the either group is removed.
        /// </summary>
        /// <typeparam name="T">The type of component to add to the either group.</typeparam>
        /// <returns>A <see cref="EitherBuilder"/> to create a either group.</returns>
        public EitherBuilder WhenRemovedEither<T>() => new EitherBuilder(this, EitherType.WhenRemoved).Or<T>();

        /// <summary>
        /// Returns a <see cref="Predicate{T}"/> representing the specified rules.
        /// </summary>
        /// <returns>The <see cref="Predicate{T}"/>.</returns>
        public Predicate<Entity> AsPredicate()
        {
            Predicate<ComponentEnum> filter = GetFilter();

            return e => filter(e.Components);
        }

        /// <summary>
        /// Returns an <see cref="EntitySet"/> with the specified rules.
        /// </summary>
        /// <returns>The <see cref="EntitySet"/>.</returns>
        public EntitySet AsSet() => new EntitySet(GetSubscriptions(out List<Func<EntityContainerWatcher, World, IDisposable>> subscriptions), _world, GetFilter(), subscriptions);

        /// <summary>
        /// Returns an <see cref="EntityMap{TKey}"/> with the specified rules.
        /// </summary>
        /// <typeparam name="TKey">The component type to use as key.</typeparam>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}.Default"/> for the type of the key.</param>
        /// <returns>The <see cref="EntityMap{TKey}"/>.</returns>
        public EntityMap<TKey> AsMap<TKey>(IEqualityComparer<TKey> comparer)
        {
            With<TKey>();

            return new EntityMap<TKey>(GetSubscriptions(out List<Func<EntityContainerWatcher, World, IDisposable>> subscriptions), _world, GetFilter(), subscriptions, comparer);
        }

        /// <summary>
        /// Returns an <see cref="EntityMap{TKey}"/> with the specified rules.
        /// </summary>
        /// <typeparam name="TKey">The component type to use as key.</typeparam>
        /// <returns>The <see cref="EntityMap{TKey}"/>.</returns>
        public EntityMap<TKey> AsMap<TKey>() => AsMap<TKey>(default);

        /// <summary>
        /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
        /// </summary>
        /// <typeparam name="TKey">The component type to use as key.</typeparam>
        /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
        /// <param name="factory">The factory used to create the <see cref="Entity"/> collection <typeparamref name="TEntities"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}.Default"/> for the type of the key.</param>
        /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
        public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>(Func<TEntities> factory, IEqualityComparer<TKey> comparer)
            where TEntities : class, ICollection<Entity>
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            With<TKey>();

            return new EntitiesMap<TKey, TEntities>(GetSubscriptions(out List<Func<EntityContainerWatcher, World, IDisposable>> subscriptions), _world, GetFilter(), subscriptions, factory, comparer);
        }

        /// <summary>
        /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
        /// </summary>
        /// <typeparam name="TKey">The component type to use as key.</typeparam>
        /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
        /// <param name="factory">The factory used to create the <see cref="Entity"/> collection <typeparamref name="TEntities"/>.</param>
        /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
        public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>(Func<TEntities> factory) where TEntities : class, ICollection<Entity> => AsMap<TKey, TEntities>(factory, default);

        /// <summary>
        /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
        /// </summary>
        /// <typeparam name="TKey">The component type to use as key.</typeparam>
        /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default <see cref="EqualityComparer{T}.Default"/> for the type of the key.</param>
        /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
        public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>(IEqualityComparer<TKey> comparer) where TEntities : class, ICollection<Entity>, new() => AsMap(() => new TEntities(), comparer);

        /// <summary>
        /// Returns an <see cref="EntitiesMap{TKey, TEntities}"/> with the specified rules.
        /// </summary>
        /// <typeparam name="TKey">The component type to use as key.</typeparam>
        /// <typeparam name="TEntities">The collection type used to store <see cref="Entity"/> sharing the same key.</typeparam>
        /// <returns>The <see cref="EntitiesMap{TKey, TEntities}"/>.</returns>
        public EntitiesMap<TKey, TEntities> AsMap<TKey, TEntities>() where TEntities : class, ICollection<Entity>, new() => AsMap<TKey, TEntities>(() => new TEntities());

        #endregion
    }
}
