   M S B A M L   `   `   `  ����F  BJiraReports, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]3clr-namespace:JiraReports.View.Reports.TimeTracking%JiraReports.View.Reports.TimeTracking  5      C    =JiraReports.View.Reports.TimeTracking.TimeTrackingOptionsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Elocal3clr-namespace:JiraReports.View.Reports.TimeTracking     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $��24 �6   �� 5      $	��110 �6    � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      .+�5   
   �� $��0��6   $��0��6   $��Start Date (Inclusive)��6-   �� 5   
   $��0��6   $��1��6   $��End Date (Exclusive)��6-   (  "System.Windows.Controls.DatePicker  5   
   $��1��6   $��0��6"   $
��Left=�62   $	��Top=�6M   $	��162��6e      SelectedDate 6q   �� *	StartDate+  5   
   $��1��6   $��1��6"   $
��Left=�62   $	��Top=�6M   $	��162��6e    6q   �� *	EndDate+�� 5   
   $��0��6   $��2��6   $��Project Category��6.   �� 5   
   -   5        $��lvProjectCategories��5      $��1��5      $��2��6     �� SelectionMode$ Multiple=�5      $	��110��5      ��5      �� *SelectableProjectCategories+��5      �� 5       .R�5!      �� ��6    �� *Category.Name+6A   5"      ��5$      �� 5%       �� 
TargetType" ��6   .�5&      ��  �� Property �@��
IsSelected �� Value 6/   ��  �� Mode$ TwoWay=� �� Path$ 
IsSelected�5'      5)      5+      5,      