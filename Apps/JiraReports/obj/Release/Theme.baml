   M S B A M L   `   `   `  ���� F  BJiraReports, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:JiraReportsJiraReports      JiraReports.ThemeR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   ControlBorderColor  ControlBorderBrush  �� Color  CloseButtonColor  CloseButtonBrush  DefaultTextColor  DefaultTextBrush  WindowBackgroundColor  WindowBackgroundBrush  PlaceholderTextColor 	 PlaceholderTextBrush 
 PrimaryColor  PrimaryBrush  PrimaryColorHilight  PrimaryBrushHilight  AccentColor  AccentBrush  AccentColorHilight  AccentBrushHilight  PrimaryGrayColor  PrimaryGrayBrush  PrimaryGrayColorHilight  PrimaryGrayBrushHilight  PrimaryWhiteColor  PrimaryWhiteBrush �� 
TargetType  PlaceHolder �� Property �� Value �� 
TargetType �� CornerRadius ��Name K� ZIndex ��Name
	 � Tag
 �� Triggers �� Binding �� Path �� Source%  System.Windows.Markup.Reference �� Value  PasswordPlaceHolder  ComboBoxTextBox ��Name  ComboBoxToggleButton � ColumnDefinitions F�Name �� Triggers P� Property P� Value �� 
TargetName �� Padding �� TextAlignment  ScrollThumbs �Name �� 
StartPoint �� EndPoint � Color � Offset � RowDefinitions ]�Name ]� Thumb  n�Name! ]� IncreaseRepeatButton" ��Name# J� Opacity$ ]� DecreaseRepeatButton% P� 
SourceName  ButtonSelectBrush  	DarkBrush& �� Angle  StyledComboBox' �� 	ClickMode( d� 	IsChecked) �� Mode* �� RelativeSource+ J� IsHitTestVisible, �� SelectionBoxItem- �� SelectionBoxItemTemplate. ��Name/ �� 
IsReadOnly0 �� 
IsReadOnly1 � AllowsTransparency2 �� IsDropDownOpen3 J� SnapsToDevicePixels4 � ActualWidth5 �� MaxDropDownHeight6 K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation7   DirectionalNavigation   MyFocusVisualStyte8 �� Bullet'  !System.Windows.VisualStateManager9   VisualStateGroups%  System.Windows.VisualStateGroup:   States   System.Windows.VisualState;   
Storyboard< �� 
TargetName= �� TargetProperty	> �� To? g� Duration	@ W� ToA W� From ! SwitchCheckBoxB J� Effect3  -System.Windows.Media.Effects.DropShadowEffectC   ColorD   
BlurRadiusE   ShadowDepthF   OpacityG   	Direction	H t� To " StandardCheckBox # FABI :� RoutedEventJ g� 	BeginTime    JiraReports.MouseDecoratorK  NameL   Child    JiraReports.ClippingBorderM   ChildN @� RadiusXO @� RadiusYP @� CenterQ �� ElementNameR ��Name $ MaterialButton   D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     !mdlclr-namespace:JiraReports  %�.  &	        &	      7   &	      &	 ,     7  &	 6     &	 E     7  &	 O     &	 ^     7  &	 h     &		 y     7  &	
 �     &	 �     7 
 &	 �     &	 �     7  &	 �     &	 �     7  &	 �     &	 �     7  &	 �     &	 
    7  &	     &	 '    7  &	 1    &	 D    7  &	 N    7  7  &	     7  7  &	 �    &	     7  7  '�� �    7  7 	 &	 1    '�� �	    7  &	 *    7  7  7  7  7  7  7  '	� �    7 
 7 
 7  7  &	! 	    7  7 
 7  7  7  7  7  7  7  &	" k    7  7  7  7 
 7  7  &	# �!    7  7  7  &	$ A'    7  7  7  ��#C9C9C9���� 8    ��#00569A���� 8    ��	Black���� 8    ��	White���� 8    ��#C9C9C9���� 8    ��	#FF2196F3���� 8    ��	#FF63B5F6���� 8    ��	#FFFF5252���� 8    ��	#FFFF8585���� 8    ��	#FF737373���� 8    ��	#FFA6A6A6���� 8    ��	#FFFFFFFF���� 8    �� " ��.���  � ��$ Center=���  � ��$ 15,0,0,0q���  � �� �� " ��.R��� $��1q�8��  $ 3��._�� .+��� $ PART_ContentHost��$��15q�#��z��$ 2��
������ �� $ 	TextProxy��$y�Hidden=�#��z���� #��z	 #��z��#��z��$ 1��$��Stretch=�$��Center=�$��	15,0,15,0q�$��0q����� " ��.���  � ��
 ���� 
 ��  �� $ Content�   *	TextProxy+  .a���  � ��8  �� " G�.�-   ��  � ��$ Center=���  � ��$ 15,0,0,0q���  � �� �� " G�.R��� $��1q�8��  $ 3��._�� .+��� $ PART_ContentHost��$��Stretch=�$��Stretch=���. $ 2���� $��HiddenPassword��$y�Hidden=�$�� ���� #��z	 #��z��$ 1��$��Stretch=�$��Center=�$��	15,0,15,0q�$��0q����� " ��.���  � ��
 ���� 
 ��  �� $ Content�   *HiddenPassword+  .a���  � ��8  �� " ��.R��� $ PART_ContentHost��|�. �� " d�.���  � �� �� " d�.R��  �� �� $��32 �.+��� $ Border��$��2��$ 0��#��z��#��z��$��2q��� $��0��$ 0��
������ 
������ $��10q�F� $ Arrow��$��1��8��  8�� $��Center=�$��Center=�"����        �@KL ��[ р��     	 P�  � {� .  . ��� $ Border�� � ��
 ������� $ Border�� � ��
 �������  � ��
 ������� $ Arrow�� � ��
 ������� " ��.���  �@J�SnapsToDevicePixels . ��  � ��$ Stretch=���  � ��$ Stretch=���  �@�OverridesDefaultStyle . ��  � �� �� " ��.R��� $��Border��$ 8q�$��0q�$ 0��
������ 8��  ._��� $
 Left=���  P�  �@��IsHighlighted . . ��� $ Border�� � ��
 ������� $ Border��8   � ���� " n�.���  � �� �� " n�.R�� $
 Grid��.+��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��
������ �� $ 
Rectangle1��$ 9��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto������ $	 1,0&�$	 1,1&�.��� $ #0174C9��$ 0Q�� $ #005899��$ 1Q� P�  �@�Tag$ 
Horizontal��. ��� $ 
Rectangle1�� � ��$
 Auto���� $ 
Rectangle1�� � ��$ 7���� " ��.���  �@��IsFlicksEnabled .  ��  � ��
 �������  � ��
 �������  � ��$ 22����  � �� �� " ��.R�� $ GridRoot��$��20��
������� �� $��0.00001* �.+��� $��0q�$ 0q�$��Stretch=�$��Stretch=�$��1q�8��  $ 9��._�]� $ 
PART_Track��$��0����. |�.   n� $  Thumb��#��z��#���  ! �� $" PageUp��$��ScrollBar.PageDownCommand��$# 0Q�|�.  $ �� $" PageDown��$��ScrollBar.PageUpCommand��$# 0Q�|�.   P� $% Thumb�� �@J�IsMouseOver . . ��� # �  $ Thumb�� � ��P� $% Thumb�� �@n�
IsDragging . . ��� # �  $ Thumb�� � ��P�  � {� .  . ��� $ Thumb�� � y�$ 	Collapsed=�P�  �@��Orientation$ 
Horizontal=�. ��� $ GridRoot�� �@�LayoutTransform �� $	& -90Q��� $ 
PART_Track�� �@�LayoutTransform �� $	& -90Q���  � ��$
 Auto����  � ��$ 12���� $ Thumb�� �@�Tag$ 
Horizontal���� $ PageDown�� � ��$ ScrollBar.PageLeftCommand���� $ PageUp�� � ��$  ScrollBar.PageRightCommand���� " ��.���  � ��8   ��  � ��8  ��  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � ��$ 50����  � ��$ 32����  � �� �� " ��.R��� $��Stretch=�$��Stretch=�$��1q�8�� $ 3��._�� .+�d� $��ToggleButton��#��z��#��z��#��z��8�� $��2��|�.  $��
0, 0, 0, 0q�$' Press=�( �� $ IsDropDownOpen�$) TwoWay=�* �� *TemplatedParent+�� $��ContentSite��+ .  #��z, #��z- #��z��$��12, 8, 30, 8q�$��Center=�$
��Left=��� $. PART_EditableTextBox��8�� $
��Left=�$��Center=�$��3,3,23,3q�|�. $y�Hidden=�#0 z/ ��Z� � $��Popup��$��Bottom=�1 . |�.  $��Slide=�#��z2 .��� $��DropDown��3 . #��z4 #��z5 .+��� $ DropDownBorder��8�� $��1q�8�� $ 3��._��� $��4,6,4,6q�3 . .���� 6 . $7 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P�  �@��
IsGrouping . . ���  � �� .  P�  �@��
IsEditable . . ���  � �� .  �� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=�
 �� " 	�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  �@�FocusVisualStyle# �   ��  � �� �� " 	�.R��� 8 � $��0q�.+�A� $��Target��$# 1Q�$��16��$��16��$��0q�$��2������ $  	#FFFFFFFF��A� $��Bullseye��$# 1Q�$��8��$��8��$��0q�$��0������ $  	#FFFFFFFF��9   CheckStates��.:   Checked��.; �� .���� $< Target��$,= &(Shape.Stroke).(SolidColorBrush.Color)�8>   $? 0:0:0.2D��� $< Bullseye��$*= $(Shape.Fill).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�W� $< Bullseye��$= Opacity�$@ 1X�$? 0:0:0.2D�W� $< Bullseye��$= Width�$A 1X�$@ 8X�$? 0:0:0.2D�W� $< Bullseye��$= Height�$A 1X�$@ 8X�$? 0:0:0.2D�  	Unchecked��.; �� .���� $< Target��$,= &(Shape.Stroke).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�W� $< Bullseye��$= Opacity�$@ 0X�$? 0:0:0.2D�  Indeterminate��.; �� .���� $< Target��$,= &(Shape.Stroke).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�W� $< Bullseye��$= Opacity�$@ 1X�$? 0:0:0.2D�._�� .+��� $��5,0,5,0q�$
��Left=�$��Center=���.  P�  � {� .  . ��� $ Bullseye�� � ��
 ������� $ Bullseye�� �@J�Opacity$ 0Q��� $ Target�� � ��
 ������� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  �@�FocusVisualStyle# �   ��  � �� �� " ��.R��� 8 � $��0q�.+��� $��Track��$��0q�$��0q�$ 0q�$��36��$��14��$ 7������ $  	#FFFFFFFF��A� $��Slider��$# 1Q�$��20��$��20��$��0q�$��1�����	#11000000���� $  	#FFFFFFFF��B   8C   $D 2Q�$E 2Q�$	F .25Q�$	G 270Q�9   CheckStates��.:   Checked��.; �� .��t� $< Slider��$= Margin�$? 0:0:0.1D�$H 16,0,0,0X��� $< Slider��$*= $(Shape.Fill).(SolidColorBrush.Color)�8>  $? 0:0:0.2D��� $< Track��$1= +(Border.Background).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�  	Unchecked��.; �� .��t� $< Slider��$= Margin�$? 0:0:0.1D�$H 	-16,0,0,0X��� $< Slider��$*= $(Shape.Fill).(SolidColorBrush.Color)�8>  $? 0:0:0.2D��� $< Track��$1= +(Border.Background).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�  Indeterminate��.; �� .��t� $< Slider��$= Margin�$? 0:0:0.1D�$H 0,0,0,0X��� $< Slider��$*= $(Shape.Fill).(SolidColorBrush.Color)�8>  $? 0:0:0.2D��� $< Track��$1= +(Border.Background).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�._�� .+��� $��5,0,5,0q�$
��Left=�$��Center=���.  P�  � {� .  . ��� $ Slider��8   � ���� $ Track��8   � ���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  �@�FocusVisualStyle# �   ��  � �� �� " ��.R��� 8 � $��0q�$��16��$��16��.+��� $��Target��$��2q�$��0q�$ 0q�$��16��$��16��$ 2������ 8    ���� 8   F� $��Check��F���� 	=  z р�[ ��� � �  	= �`�� `�S р�[ @]� Ѡ%& `�� � 	=  z 	$# 0Q�8�� $��0��$��0q�$��16��$��16��9   CheckStates��.:   Checked��.; �� .���� $< Target��$1= +(Border.Background).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�W� $< Check��$= Opacity�$@ 1X�$? 0:0:0.2D�t� $< Target��$= BorderThickness�$H 0X�$? 0:0:0.2D�  	Unchecked��.; �� .���� $< Target��$1= +(Border.Background).(SolidColorBrush.Color)�8>  $? 0:0:0.2D��� $< Target��$2= ,(Border.BorderBrush).(SolidColorBrush.Color)�8>  $? 0:0:0.2D�W� $< Check��$= Opacity�$@ 0X�$? 0:0:0.2D�t� $< Target��$= BorderThickness�$H 2X�$? 0:0:0.2D�  Indeterminate��.; �� ._�� .+��� $��5,0,5,0q�$
��Left=�$��Center=���.  P�  � {� .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@��TextAlignment$ Center=���  � �� �� " �� :� $I Click��.[��� .���� .��W� $< Splash��$= Opacity�$@ 0X�$? 0:0:0D�W� $< 	FABShadow��$= ShadowDepth�$@ 2X�$? 0:0:0.2D�W� $< 	FABShadow��$= ShadowDepth�$J 0:0:0.2X�$@ 4X�$? 0:0:0.2D�W� $< Splash��$= Opacity�$@ 1X�$? 0:0:.25D�W� $< 
SplashWave��$= RadiusX�$A 1X�$	@ 500X�$? 0:0:1.5D�W� $< 
SplashWave��$= RadiusY�$A 1X�$	@ 500X�$? 0:0:1.5D�W� $< Splash��$= Opacity�$A 1X�$@ 0X�$J 0:0:1X�$? 0:0:.5D�:� $I 
MouseEnter��.[��� .���� .��W� $< 	FABShadow��$= ShadowDepth�$@ 4X�$? 0:0:0.2D�W� $< 	FABShadow��$= 
BlurRadius�$@ 4X�$? 0:0:0.2D�:� $I 
MouseLeave��.[��� .���� .��W� $< 	FABShadow��$= ShadowDepth�$@ 1X�$? 0:0:0.2D�W� $< 	FABShadow��$= 
BlurRadius�$@ 1X�$? 0:0:0.2D�.R�� .+��� $��56��$��56��$ 28��8��  $��0q�$ 1��B   	FABShadow��$C #C9C9C9��$	G 270Q�$D 1Q�$F 1Q�$E 1Q�  $K mouseTracker��$��56��$��56��$ 2��.L   $��56��$��56��$ 28��8�� $��0q�$ 2��}�. .M � .+�F� $ Splash��$# 0Q�$��0��8�� ��@� 
SplashWave��$	N 120Q�$	O 120Q�P �� $Q mouseTracker��$ MousePosition��� $R contentPresenter��#��z��#��z��#��z��#��z��#��z���� " ��.���  �@J�SnapsToDevicePixels . ��  �@��TextAlignment$ Center=���  � �� �� " �� :� $I Click��.[��� .���� .��W� $< Splash��$= Opacity�$@ 0X�$? 0:0:0D�W� $< 	FABShadow��$= ShadowDepth�$@ 2X�$? 0:0:0.2D�W� $< 	FABShadow��$= ShadowDepth�$J 0:0:0.2X�$@ 4X�$? 0:0:0.2D�W� $< Splash��$= Opacity�$@ 1X�$? 0:0:.25D�W� $< 
SplashWave��$= RadiusX�$A 1X�$	@ 500X�$? 0:0:1.5D�W� $< 
SplashWave��$= RadiusY�$A 1X�$	@ 500X�$? 0:0:1.5D�W� $< Splash��$= Opacity�$A 1X�$@ 0X�$J 0:0:1X�$? 0:0:.5D�:� $I 
MouseEnter��.[��� .���� .��W� $< 	FABShadow��$= ShadowDepth�$@ 4X�$? 0:0:0.2D�W� $< 	FABShadow��$= 
BlurRadius�$@ 4X�$? 0:0:0.2D�:� $I 
MouseLeave��.[��� .���� .��W� $< 	FABShadow��$= ShadowDepth�$@ 1X�$? 0:0:0.2D�W� $< 	FABShadow��$= 
BlurRadius�$@ 1X�$? 0:0:0.2D�.R�� -   .+��� $ 2��8��  $��0q�$ 1��B   	FABShadow��$C #C9C9C9��$	G 270Q�$D 1Q�$F 1Q�$E 1Q�  $K mouseTracker��$ 2��.L   $ 2��8�� $��0q�$ 2��}�. .M � $ 
ButtonGrid��.+�F� $ Splash��$# 0Q�$��0��8�� ��@� 
SplashWave��$N 0Q�$O 0Q�P �� $Q mouseTracker��$ MousePosition��� $R contentPresenter��#��z��#��z��#��z��#��z��#��z��