//-----------------------------------------------
// XUI - Base.cs
// Copyright (C) Peter Reid. All rights reserved.
//-----------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

// TODO - DirtyMatrix/Color flags - CalculateTransform/Color are the two biggest perf hotspots for more complex screens atm!
// TODO - InheritS/R/T flags
// TODO - check for parent loops in Init

namespace UI
{

// E_WidgetFlag
public enum E_WidgetFlag
{
	UseMatrix			= 0x01,
	InheritAlpha		= 0x02,
	InheritIntensity	= 0x04,
};

// class WidgetBase
public class WidgetBase
{
	// WidgetBase
	public WidgetBase()
	{
		IsActive = true;
		IsSelected = true;

		Name = null;
		
		RenderPass = 0;
		Layer = 0;

		Position = new Vector3( 0.0f, 0.0f, 0.0f );
		Size = new Vector3( 0.0f, 0.0f, 0.0f );
		Scale = new Vector2( 1.0f, 1.0f );
		Rotation = new Vector3( 0.0f, 0.0f, 0.0f );

		Align = E_Align.TopLeft;

		ParentWidget = null;
		ParentAttach = E_Align.None;
		
		Children = new List< WidgetBase >();
		
		Alpha = 1.0f;
		Intensity = 1.0f;
		ColorBaseName = null;
		ColorBase = Color.Magenta;

		RenderStateName = null;
		RenderState = new RenderState( (int)E_Effect.MultiTexture1, E_BlendState.AlphaBlend );

		Textures = new List< SpriteTexture >();

		Timelines = new List< Timeline >();
		
		AlphaFinal = Alpha;
		IntensityFinal = Intensity;
		ColorFinal = ColorBase;

		Flags = (int)E_WidgetFlag.InheritAlpha | (int)E_WidgetFlag.InheritIntensity;
		TransformMatrix = Matrix.Identity;

		UpY = 1.0f;
	}

	// Copy
	public WidgetBase Copy()
	{
		WidgetBase o = (WidgetBase)Activator.CreateInstance( GetType() );

		CopyTo( o );

		return o;
	}

	// CopyTo
	protected virtual void CopyTo( WidgetBase o )
	{
		o.IsActive = IsActive;
		o.IsSelected = IsSelected;

		o.Name = Name;

		o.RenderPass = RenderPass;
		o.Layer = Layer;

		o.Position = Position;
		o.Size = Size;
		o.Scale = Scale;
		o.Rotation = Rotation;

		o.Align = Align;

		// ParentWidget - doesn't copy over
		o.ParentAttach = ParentAttach;

		// Children - don't copy over

		o.Alpha = Alpha;
		o.Intensity = Intensity;
		o.ColorBaseName = ColorBaseName;
		o.ColorBase = ColorBase;

		o.RenderStateName = RenderStateName;
		o.RenderState = RenderState;

		o.Textures.Capacity = Textures.Count;

		for ( int i = 0; i < Textures.Count; ++i )
			o.Textures.Add( Textures[ i ] );

		o.Timelines.Capacity = Timelines.Count;

		for ( int i = 0; i < Timelines.Count; ++i )
			o.Timelines.Add( Timelines[ i ].Copy() );

		o.AlphaFinal = AlphaFinal;
		o.IntensityFinal = IntensityFinal;
		o.ColorFinal = ColorFinal;

		o.Flags = Flags;
		o.TransformMatrix = TransformMatrix;

		o.UpY = UpY;
	}

	// CopyAndAdd
	public WidgetBase CopyAndAdd( Screen screen )
	{
		return CopyAndAdd_Aux( screen, this );
	}

	private WidgetBase CopyAndAdd_Aux( Screen screen, WidgetBase widget )
	{
		WidgetBase o = widget.Copy();
		screen.Add( o );

		for ( int i = 0; i < widget.Children.Count; ++i )
		{
			WidgetBase oo = CopyAndAdd_Aux( screen, widget.Children[ i ] );
			oo.Parent( o );
		}

		return o;
	}

	// FindChild
	public WidgetBase FindChild( string name )
	{
		return FindChild_Aux( name, this );
	}

	private WidgetBase FindChild_Aux( string name, WidgetBase widget )
	{
		for ( int i = 0; i < widget.Children.Count; ++i )
		{
			WidgetBase o = widget.Children[ i ];

			if ( ( o.Name != null ) && ( o.Name.Equals( name ) ) )
				return o;

			WidgetBase oo = FindChild_Aux( name, o );

			if ( oo != null )
				return oo;
		}

		return null;
	}

	// AddTexture
	public void AddTexture( string name, ref Vector2 puv, ref Vector2 suv )
	{
		Textures.Add( new SpriteTexture( name, ref puv, ref suv ) );
	}

	public void AddTexture( string name, float pu, float pv, float su, float sv )
	{
		Textures.Add( new SpriteTexture( name, pu, pv, su, sv ) );
	}

	public void AddTexture( string name )
	{
		Textures.Add( _UI.Store_Texture.Get( name ) );
	}

	// ChangeTexture
	public void ChangeTexture( int slot, string name, ref Vector2 puv, ref Vector2 suv )
	{
		Textures[ slot ] = new SpriteTexture( name, ref puv, ref suv );
	}

	public void ChangeTexture( int slot, string name, float pu, float pv, float su, float sv )
	{
		Textures[ slot ] = new SpriteTexture( name, pu, pv, su, sv );
	}

	public void ChangeTexture( int slot, string name )
	{
		Textures[ slot ] = _UI.Store_Texture.Get( name );
	}

	public void ChangeTexture( int slot, ref SpriteTexture texture )
	{
		Textures[ slot ] = texture;
	}

	// GetTexture
	public SpriteTexture GetTexture( int slot )
	{
		return Textures[ slot ];
	}

	// Init
	public void Init()
	{
		if ( ColorBaseName != null )
			ColorBase = _UI.Store_Color.Get( ColorBaseName );

		if ( RenderStateName != null )
			RenderState = _UI.Store_RenderState.Get( RenderStateName );

		for ( int i = 0; i < Timelines.Count; ++i )
			Timelines[ i ].Bind( this );

		UpY = _UI.Sprite.IsRenderPass3D( RenderPass ) ? -1.0f : 1.0f;

		OnInit();
	}

	// OnInit
	protected virtual void OnInit()
	{
		//
	}

	// PostInit
	public void PostInit()
	{
		OnPostInit();
	}

	// OnPostInit
	protected virtual void OnPostInit()
	{
		//
	}

	// ProcessInput
	public void ProcessInput( Input input )
	{
		if ( !IsActive || !IsSelected )
			return;

		OnProcessInput( input );
	}

	// OnProcessInput
	protected virtual void OnProcessInput( Input input )
	{
		//
	}

	// Update
	public void Update( float frameTime )
	{
		if ( !IsActive )
			return;

		UpdateTimelines( frameTime );

		OnUpdate( frameTime );
	}

	// OnUpdate
	protected virtual void OnUpdate( float frameTime )
	{
		//
	}

	// Render
	public void Render()
	{
		if ( !IsActive )
			return;

		CalculateColor();
		CalculateTransform();

		OnRender();
	}

	// OnRender
	protected virtual void OnRender()
	{
		//
	}

	// CalculateTransform
	private void CalculateTransform()
	{
		if ( ( ParentWidget == null ) && ( Scale.X == 1.0f ) && ( Scale.Y == 1.0f ) && ( Rotation.X == 0.0f ) && ( Rotation.Y == 0.0f ) && ( Rotation.Z == 0.0f ) )
			FlagClear( E_WidgetFlag.UseMatrix );
		else
		{
			FlagSet( E_WidgetFlag.UseMatrix );

			Matrix transformMatrix;
			CalculateTransform( out transformMatrix );

			for ( WidgetBase t = this, p = ParentWidget; p != null; t = p, p = p.ParentWidget )
			{
				Matrix transformMatrixParent;
				p.CalculateTransform( out transformMatrixParent );

				transformMatrix.Translation = transformMatrix.Translation 
					+ new Vector3( new Vector2( p.Size.X, p.Size.Y ) * ( _UI.Sprite.GetVertexOffsetAligned( RenderPass, t.ParentAttach ) - _UI.Sprite.GetVertexOffsetAligned( p.RenderPass, p.Align ) ), 0.0f );

				transformMatrix *= transformMatrixParent;
			}

			TransformMatrix = transformMatrix;
		}
	}

	private void CalculateTransform( out Matrix matrix )
	{
		matrix = Matrix.Identity;
		matrix *= Matrix.CreateScale( Scale.X, Scale.Y, 1.0f );
		matrix *= Matrix.CreateFromYawPitchRoll( MathHelper.ToRadians( Rotation.Y ), MathHelper.ToRadians( Rotation.X ), MathHelper.ToRadians( Rotation.Z ) );
		matrix.Translation = Position;
	}

	// CalculateColor
	private void CalculateColor()
	{
		AlphaFinal = Alpha;
		IntensityFinal = Intensity;

		bool doAlpha = IsFlagSet( E_WidgetFlag.InheritAlpha );
		bool doIntensity = IsFlagSet( E_WidgetFlag.InheritIntensity );
		
		for ( WidgetBase p = ParentWidget; p != null; p = p.ParentWidget )
		{
			if ( doAlpha )
				AlphaFinal *= p.Alpha;

			if ( doIntensity )
			{
				if ( p.Intensity > 1.0f )
					IntensityFinal += ( p.Intensity - 1.0f );
				else
					IntensityFinal *= p.Intensity;
			}

			doAlpha &= p.IsFlagSet( E_WidgetFlag.InheritAlpha );
			doIntensity &= p.IsFlagSet( E_WidgetFlag.InheritIntensity );

			if ( !doAlpha && !doIntensity )
				break;
		}

		ColorFinal.Set( ref ColorBase, AlphaFinal, IntensityFinal );

		if ( RenderState.BlendState == E_BlendState.AlphaBlend )
			ColorFinal.ToPremultiplied();
	}

	// AddTimeline
	public void AddTimeline( Timeline timeline )
	{
		Timelines.Add( timeline );
	}

	public void AddTimeline( string name )
	{
		Timeline timeline = _UI.Store_Timeline.Get( name );

		if ( timeline != null )
			Timelines.Add( timeline );
	}

	// UpdateTimelines
	private void UpdateTimelines( float frameTime )
	{
		for ( int i = 0; i < Timelines.Count; ++i )
			Timelines[ i ].Update( frameTime );
	}

	// TimelineActive
	public void TimelineActive( string name, bool value, bool children )
	{
		for ( int i = 0; i < Timelines.Count; ++i )
		{
			Timeline timeline = Timelines[ i ];

			if ( timeline.Name.Equals( name ) )
				timeline.SetActive( value );
		}

		if ( children )
			for ( int i = 0; i < Children.Count; ++i )
				Children[ i ].TimelineActive( name, value, children );
	}

	// TimelineReset
	public void TimelineReset( string name, bool value, float time, bool children )
	{
		for ( int i = 0; i < Timelines.Count; ++i )
		{
			Timeline timeline = Timelines[ i ];

			if ( timeline.Name.Equals( name ) )
				timeline.Reset( time, value );
		}

		if ( children )
			for ( int i = 0; i < Children.Count; ++i )
				Children[ i ].TimelineReset( name, value, time, children );
	}

	// Active
	public void Active( bool value, bool children )
	{
		IsActive = value;

		OnActive( value );

		if ( children )
			for ( int i = 0; i < Children.Count; ++i )
				Children[ i ].Active( value, children );
	}

	public void Active( bool value )
	{
		Active( value, false );
	}

	public bool Active()
	{
		return IsActive;
	}

	// OnActive
	protected virtual void OnActive( bool value )
	{
		//
	}

	// Selected
	public void Selected( bool value, bool children, bool timelineMessage )
	{
		IsSelected = value;

		if ( timelineMessage )
			TimelineActive( "selected", value, false );

		OnSelected( value );

		if ( children )
			for ( int i = 0; i < Children.Count; ++i )
				Children[ i ].Selected( value, children, timelineMessage );
	}

	public void Selected( bool value )
	{
		Selected( value, false, false );
	}

	public bool Selected()
	{
		return IsSelected;
	}

	// OnSelected
	protected virtual void OnSelected( bool value )
	{
		//
	}

	// Parent
	public void Parent( WidgetBase widget )
	{
		if ( ParentWidget != null )
			ParentWidget.Children.Remove( this );
		
		ParentWidget = widget;
		
		if ( ParentWidget != null )
			ParentWidget.Children.Add( this );
	}

	// FlagSet
	public void FlagSet( E_WidgetFlag flag )
	{
		Flags |= (int)flag;
	}

	// FlagClear
	public void FlagClear( E_WidgetFlag flag )
	{
		Flags &= ~(int)flag;
	}

	// IsFlagSet
	public bool IsFlagSet( E_WidgetFlag flag )
	{
		return ( ( Flags & (int)flag ) != 0 );
	}

	//
	protected bool						IsActive;
	protected bool						IsSelected;

	public string						Name;

	public int							RenderPass;
	public int							Layer;

	public Vector3						Position;
	public Vector3						Size;
	public Vector2						Scale;
	public Vector3						Rotation;

	public E_Align						Align;

	protected WidgetBase				ParentWidget;
	public    E_Align					ParentAttach;

	protected List< WidgetBase >		Children;
	
	public float						Alpha;
	public float						Intensity;
	public string						ColorBaseName;
	public SpriteColors					ColorBase;

	public string						RenderStateName;
	public RenderState					RenderState;

	protected List< SpriteTexture >		Textures;

	protected List< Timeline >			Timelines;

	protected float						AlphaFinal;
	protected float						IntensityFinal;
	protected SpriteColors				ColorFinal;

	private int							Flags;
	public  Matrix						TransformMatrix;

	public  float						UpY;
	//
};

}; // namespace UI
