   M S B A M L   `   `   `  ����   JiraReports!    JiraReports.View.ReportPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     
�������.��]� ��. .+�o� $	��Top=�.@�m� $��Reports��.@�m� $��
New Report������ *ReportTypes+����   �� 
TargetType"  m�.���  �� Property �@m�Command �� Value �� *NewReportCommand+ �� Triggers ��  �� Binding �� *	IsVisible+ �� Value	 False.a���  � y�$ 	Collapsed=����� .R��� ���� *Name+�� ���� *	Reports+ �� SelectedIndex �� *SelectedIndex+���� .R���  �� Orientation$ 
Horizontal=�.+��� ���� *Name+�� $��5q�$��0q�$��Right=�$��Center=�$��12��$��12��   PrimaryBrush#��[  ���� *CloseReportCommand+.���� $-��'/JiraReports;component/Images/close.png��$��Stretch=�$��Stretch=� �� ContentTemplate �� .R�� 	 � RowDefinitions	 �� $
��1.5* ��� $��5 ��� $��4* �.+�]� $��0����. .+�� $��Right=�$	��130��.+���   MaterialButton#��[ $��Right=�$��Bottom=�$��5q�$	��120������ *RunReportCommand+.����   PrimaryWhiteBrush#��[ $��12(�$
��Bold"�$��5q�
 �� Padding$
 0q�
Run Report�� $
��Left=� � DataContext �� *OptionsViewModel+����  �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType$ 	DockPanelL� �� Path$ DataContext.OptionsPage��� $��1��$��5��$��Stretch=��� $��2�� �� *DisplayViewModel+����  �� $ FindAncestor=�$
 GridL�$ DataContext.DisplayPage�