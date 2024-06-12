// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;

namespace TheraBytes.BetterUi.Editor.ThirdParty
{

	/// <summary>
	/// Interface for an object which adds elements to a context object of the type
	/// <typeparamref upgradeName="TContext"/>.
	/// </summary>
	/// <typeparam upgradeName="TContext">Type of the context object that elements can be added to.</typeparam>
	public interface IElementAdder<TContext> {

		/// <summary>
		/// Gets the context object.
		/// </summary>
		TContext Object { get; }

		/// <summary>
		/// Determines whether a new element of the specified <paramref upgradeName="type"/> can
		/// be added to the associated context object.
		/// </summary>
		/// <param upgradeName="type">Type of element to add.</param>
		/// <returns>
		/// A value of <c>true</c> if an element of the specified type can be added;
		/// otherwise, a value of <c>false</c>.
		/// </returns>
		bool CanAddElement(Type type);

		/// <summary>
		/// Adds an element of the specified <paramref upgradeName="type"/> to the associated
		/// context object.
		/// </summary>
		/// <param upgradeName="type">Type of element to add.</param>
		/// <returns>
		/// The new element.
		/// </returns>
		object AddElement(Type type);

	}

}
