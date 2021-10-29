declare @StartDate DateTime = '2019-01-01 00:00:00';
declare @EndDate DateTime = '2019-06-30 23:59:59';

--adjjust enetered datetime to conver to utc time, which is used in the Jira db fields
--this assumes that entered times above are in US Eastern timezone 
set @StartDate = dateadd(hour, 4, @StartDate);
set @EndDate = dateadd(hour, 4, @EndDate);

select
       p.pkey + '-' + convert(nvarchar(20), i.issuenum) as IssueId,
       ep.pkey + '-' + convert(nvarchar(20), epic.issuenum) as EpicIssueId,
       i.SUMMARY as Summary,
       epic.SUMMARY as EpicSummary,
          case when capex.STRINGVALUE = 10800 then 'Yes' else 'No' end as Capex,
       it.pname as IssueType,
       cu.first_name as FirstName,
       cu.last_name as LastName,
       cu.display_name as DisplayName,
       p.pname as ProjectName,
       w.timeworked / 3600 as [Hours],
       w.Created,
       w.Updated,
       w.StartDate,
       substring((select '|' + c.cname from nodeassociation na join component c on na.SINK_NODE_ID = c.id where na.SINK_NODE_ENTITY = 'Component' and na.SOURCE_NODE_ID = i.id order by c.cname for xml path ('')), 2, 1000) as Components,
       substring((select '|' + p.vname from nodeassociation na join projectversion p on na.SINK_NODE_ID = p.id where na.SINK_NODE_ENTITY = 'Version' and na.SOURCE_NODE_ID = i.id order by p.vname for xml path ('')), 2, 1000) as FixVersions,
   case when cf.stringvalue = 10108 then 'Qualified' when cf.stringvalue = 10109 then 'Non-Qualified' else 'None' end as TaxCredit     
--case 
--when cf.stringvalue = 10108 then 'Qualified'
--when cf.stringvalue = 10109 then 'Non-Qualified'
--else 'None' 
--end as TaxCredit

       from 
                     worklog w
       left join 
                     jiraissue i on w.issueid = i.ID
       left join 
                     issuetype it on i.issuetype = it.id
       left join
                     issuelink sub_parent on sub_parent.DESTINATION = i.ID and sub_parent.LINKTYPE = 10100
       left join
                     issuelink epl on epl.DESTINATION = isnull(sub_parent.SOURCE, i.ID) and epl.LINKTYPE = 10200
       left join
                     jiraissue epic on epic.ID = epl.SOURCE
       left join 
                     project p on i.PROJECT = p.ID
       left join 
                     project ep on epic.PROJECT = ep.ID     
       left join 
                     app_user au on w.author = au.lower_user_name
       left join 
                     jira.dbo.cwd_user cu on au.id = cu.id
       left join
                     jira.dbo.customfieldvalue cf on i.ID = cf.issue and cf.customfield = 10304
       left join
                     jira.dbo.customfieldvalue capex on epic.ID = capex.issue and capex.customfield = 11500

       where 
                     --(select 
                     --     count(*) 
                     -- from 
                     --     nodeassociation na 
               --    join component c on na.SINK_NODE_ID = c.id 
                     -- where 
                     --     na.SINK_NODE_ENTITY = 'Component' and na.SOURCE_NODE_ID = i.id) = 0 
                     --and
                     w.CREATED between @StartDate and @EndDate
       --and              
       --                   cf.customfield = 10304
       --and
       --                 cf.stringvalue = 10108
          order by
                     w.CREATED;


