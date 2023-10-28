//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v12.2.0+173d8dc
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	/// <summary>Home</summary>
	[PublishedModel("home")]
	public partial class Home : PublishedContentModel, ICommonSettings, IHeroSection
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		public new const string ModelTypeAlias = "home";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<Home, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public Home(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Middle Content
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("middleContent")]
		public virtual global::Umbraco.Cms.Core.Models.Blocks.BlockGridModel MiddleContent => this.Value<global::Umbraco.Cms.Core.Models.Blocks.BlockGridModel>(_publishedValueFallback, "middleContent");

		///<summary>
		/// Top Button Link
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("topButtonLink")]
		public virtual global::Umbraco.Cms.Core.Models.Link TopButtonLink => this.Value<global::Umbraco.Cms.Core.Models.Link>(_publishedValueFallback, "topButtonLink");

		///<summary>
		/// Top Header Text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("topHeaderText")]
		public virtual string TopHeaderText => this.Value<string>(_publishedValueFallback, "topHeaderText");

		///<summary>
		/// Top Text Block
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("topTextBlock")]
		public virtual string TopTextBlock => this.Value<string>(_publishedValueFallback, "topTextBlock");

		///<summary>
		/// Display Journal
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[ImplementPropertyType("displayJournal")]
		public virtual bool DisplayJournal => global::Umbraco.Cms.Web.Common.PublishedModels.CommonSettings.GetDisplayJournal(this, _publishedValueFallback);

		///<summary>
		/// Hide From Navigation
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[ImplementPropertyType("hideFromNavigation")]
		public virtual bool HideFromNavigation => global::Umbraco.Cms.Web.Common.PublishedModels.CommonSettings.GetHideFromNavigation(this, _publishedValueFallback);

		///<summary>
		/// Hide Hero Section
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[ImplementPropertyType("hideHeroSection")]
		public virtual bool HideHeroSection => global::Umbraco.Cms.Web.Common.PublishedModels.CommonSettings.GetHideHeroSection(this, _publishedValueFallback);

		///<summary>
		/// Background Image
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("backgroundImage")]
		public virtual global::Umbraco.Cms.Core.Models.MediaWithCrops BackgroundImage => global::Umbraco.Cms.Web.Common.PublishedModels.HeroSection.GetBackgroundImage(this, _publishedValueFallback);

		///<summary>
		/// Include Search Bar
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[ImplementPropertyType("includeSearchBar")]
		public virtual bool IncludeSearchBar => global::Umbraco.Cms.Web.Common.PublishedModels.HeroSection.GetIncludeSearchBar(this, _publishedValueFallback);

		///<summary>
		/// Text Block
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.2.0+173d8dc")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("textBlock")]
		public virtual global::Umbraco.Cms.Core.Strings.IHtmlEncodedString TextBlock => global::Umbraco.Cms.Web.Common.PublishedModels.HeroSection.GetTextBlock(this, _publishedValueFallback);
	}
}
