   M S B A M L   `   `   `  ����F  BJiraReports, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:JiraReports.CommonJiraReports.Common  5          JiraReports.View.Login   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      _ [Microsoft.Xaml.Behaviors, Version=1.0.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  5      0	behaviors clr-namespace:JiraReports.Common  5      
�������.��5
      �   � ColumnDefinitions  5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6    � RowDefinitions 5   
   �� 5      $��2* �6   �� 5      $��50 �6   �� 5      $��50 �6   �� 5      $��2* �6   .+�5   
   �� $��0��6   $��1��6   $��26��6-   $��Bottom=�69   $��5q�6T   $��Invalid Login��6_   
���  ��$��Center=�6�   y�6�   �� *InvalidLoginVisible+�� 5   
   $��0��6   $��1��6   $��26��6-   $��Bottom=�69   $��5q�6T   $��
Logging In��6_   
��� � �$��Center=�6�   y�6�   �� *LoggingInVisible+�� 5   
      PlaceHolder#��[  6,   
 � Tag$ Username��6Q   $��5,3,5,2q�6`   $��1��6q   $��1��6~   $��0��6�   ��6   �� *
Username+6�   G� 5   
     PasswordPlaceHolder#��[ 6   $ Password��6C   $��5,3,5,2q�6R   $��2��6c   $��1��6p   $��1��6�   *  $Microsoft.Xaml.Behaviors.Interaction   Triggers 5      +  %Microsoft.Xaml.Behaviors.EventTrigger  5         	EventName$ PasswordChanged��6!   *  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 5      (   "JiraReports.Common.PropertyManager     SourceProperty$ Password��60      Binding 5      ��  �� Path$ Password�	 �� Mode$	 TwoWay=�
 �� UpdateSourceTrigger$
 PropertyChanged=�5      5       �� 5!   
    �� 	IsDefault .   MaterialButton#��[ 6"   $	��120��6J   $��5q�6V   $��3��6a   $��1��6n   $��Right=�6~   $	��Top=�6�   $��2��6�   ��5"      �� *LoginCommand+.��5#      ��   PrimaryWhiteBrush#��[ 6   $��14(�6H   $
��Bold"�6V   $��10q�6h    �� Padding$ 0q�6t   Login6�   5$      5%      5'      