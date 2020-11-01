﻿using System;
using System.Diagnostics.CodeAnalysis;
using DefaultEcs.Technical.Command;

namespace DefaultEcs.Command
{
    /// <summary>
    /// Represents an <see cref="Entity"/> on which to create commands to record in a <see cref="EntityCommandRecorder"/>.
    /// </summary>
    public readonly ref struct EntityRecord
    {
        #region Fields

        private readonly EntityCommandRecorder _recorder;
        private readonly int _offset;

        #endregion

        #region Initialisation

        internal EntityRecord(EntityCommandRecorder recorder, int offset)
        {
            _recorder = recorder;
            _offset = offset;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Enables the corresponding <see cref="Entity"/> so it can appear in <see cref="EntitySet"/>.
        /// This command takes 5 bytes.
        /// </summary>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void Enable() => _recorder.WriteCommand(new EntityOffsetCommand(CommandType.Enable, _offset));

        /// <summary>
        /// Disables the corresponding <see cref="Entity"/> so it does not appear in <see cref="EntitySet"/>.
        /// This command takes 5 bytes.
        /// </summary>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void Disable() => _recorder.WriteCommand(new EntityOffsetCommand(CommandType.Disable, _offset));

        /// <summary>
        /// Enables the corresponding <see cref="Entity"/> component of type <typeparamref name="T"/> so it can appear in <see cref="EntitySet"/>.
        /// Does nothing if corresponding <see cref="Entity"/> does not have a component of type <typeparamref name="T"/>.
        /// This command takes 9 bytes.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void Enable<T>() => _recorder.WriteCommand(new EntityOffsetComponentCommand(CommandType.EnableT, ComponentCommands.ComponentCommand<T>.Index, _offset));

        /// <summary>
        /// Disables the corresponding <see cref="Entity"/> component of type <typeparamref name="T"/> so it does not appear in <see cref="EntitySet"/>.
        /// This command takes 9 bytes.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void Disable<T>() => _recorder.WriteCommand(new EntityOffsetComponentCommand(CommandType.DisableT, ComponentCommands.ComponentCommand<T>.Index, _offset));

        /// <summary>
        /// Sets the value of the component of type <typeparamref name="T"/> on the corresponding <see cref="Entity"/>.
        /// For a blittable component, this command takes 9 bytes + the size of the component.
        /// For non blittable component, this command takes 13 bytes and may cause some allocation because of boxing on struct component type.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="component">The value of the component.</param>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        [SuppressMessage("Performance", "RCS1242:Do not pass non-read-only struct by read-only reference.")]
        public void Set<T>(in T component = default) => _recorder.WriteSetCommand(_offset, component);

        /// <summary>
        /// Sets the value of the component of type <typeparamref name="T"/> on the corresponding <see cref="Entity"/> to the same instance of an other <see cref="EntityRecord"/>.
        /// This command takes 13 bytes.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="reference">The other <see cref="EntityRecord"/> used as reference.</param>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void SetSameAs<T>(in EntityRecord reference) => _recorder.WriteCommand(new EntityReferenceOffsetComponentCommand(CommandType.SetSameAs, ComponentCommands.ComponentCommand<T>.Index, _offset, reference._offset));

        /// <summary>
        /// Removes the component of type <typeparamref name="T"/> on the corresponding <see cref="Entity"/>.
        /// This command takes 9 bytes.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void Remove<T>() => _recorder.WriteCommand(new EntityOffsetComponentCommand(CommandType.Remove, ComponentCommands.ComponentCommand<T>.Index, _offset));

        /// <summary>
        /// Notifies the value of the component of type <typeparamref name="T"/> has changed on the corresponding <see cref="Entity"/>.
        /// This command takes 9 bytes.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void NotifyChanged<T>() => _recorder.WriteCommand(new EntityOffsetComponentCommand(CommandType.NotifyChanged, ComponentCommands.ComponentCommand<T>.Index, _offset));

        /// <summary>
        /// Clean the corresponding <see cref="Entity"/> of all its components.
        /// The current <see cref="EntityRecord"/> should not be used again after calling this method.
        /// This command takes 5 bytes.
        /// </summary>
        /// <exception cref="InvalidOperationException">Command buffer is full.</exception>
        public void Dispose() => _recorder.WriteCommand(new EntityOffsetCommand(CommandType.Dispose, _offset));

        #endregion
    }
}
