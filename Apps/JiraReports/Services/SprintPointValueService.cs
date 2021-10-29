﻿using JiraReports.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    [SingleInstance(typeof(ISprintPointValueService))]
    public class SprintPointValueService : ISprintPointValueService
    {
        public static string SprintPointValues = @"0	2.8624326799283	0.4403395212506106497313141182	0	7211
2.8624326799283	5.7248653598566	0.6378847093307278944797264289	1	3235
5.7248653598566	8.5872980397849	0.7508549096238397655105031754	2	1850
8.5872980397849	11.4497307197132	0.8122862725940400586223742062	3	1006
11.4497307197132	14.3121633996415	0.8603444064484611626770884221	4	787
14.3121633996415	17.1745960795698	0.8946629213483146067415730337	5	562
17.1745960795698	20.0370287594981	0.9186003908158280410356619443	6	392
20.0370287594981	22.8994614394264	0.9319736199316072300928187592	7	219
22.8994614394264	25.7618941193547	0.9441255495847581827063996092	8	199
25.7618941193547	28.6243267992830	0.9534684904738641914997557401	9	153
28.6243267992830	31.4867594792113	0.9616511968734733756717147044	10	134
31.4867594792113	34.3491921591396	0.9680019540791402051783097215	11	104
34.3491921591396	37.2116248390679	0.9733146067415730337078651685	12	87
37.2116248390679	40.0740575189962	0.9780776746458231558378114314	13	78
40.0740575189962	42.9364901989245	0.9816805080605764533463605276	14	59
42.9364901989245	45.7989228788528	0.9843673668783585735222276502	15	44
45.7989228788528	48.6613555587811	0.987359550561797752808988764	16	49
48.6613555587811	51.5237882387094	0.9892525647288715192965315095	17	31
51.5237882387094	54.3862209186377	0.9906570591108939912066438691	18	23
54.3862209186377	57.2486535985660	0.9918172936003908158280410357	19	19
57.2486535985660	60.1110862784943	0.9930385930630190522716170005	20	20
60.1110862784943	62.9735189584226	0.9941988275525158768930141671	21	19
62.9735189584226	65.8359516383509	0.994687347337567171470444553	22	8
65.8359516383509	68.6983843182792	0.9951758671226184660478749389	23	8
68.6983843182792	71.5608169982075	0.9958475818270639960918417196	24	11
71.5608169982075	74.4232496781358	0.9963361016121152906692721055	25	8
74.4232496781358	77.2856823580641	0.9968246213971665852467024915	26	8
77.2856823580641	80.1481150379924	0.9970688812896922325354176844	27	4
80.1481150379924	83.0105477179207	0.9973131411822178798241328774	28	4
83.0105477179207	85.8729803978490	0.9974352711284807034684904739	29	2
85.8729803978490	88.7354130777773	0.9978016609672691744015632633	30	6
88.7354130777773	91.5978457577056	0.9980459208597948216902784563	31	4
91.5978457577056	94.4602784376339	0.9982901807523204689789936492	32	4
94.4602784376339	97.3227111175622	0.9985344406448461162677088422	33	4
97.3227111175622	100.1851437974905	0.9986565705911089399120664387	34	2
100.1851437974905	103.0475764774188	0.9988397655105031753786028334	35	3
105.9100091573471	108.7724418372754	0.9990229604298974108451392281	37	3
108.7724418372754	111.6348745172037	0.9991450903761602344894968246	38	2
111.6348745172037	114.4973071971320	0.9992061553492916463116756229	39	1
114.4973071971320	117.3597398770603	0.9992672203224230581338544211	40	1
117.3597398770603	120.2221725569886	0.9993893502686858817782120176	41	2
123.0846052369169	125.9470379168452	0.9994504152418172936003908158	43	1
128.8094705967735	131.6719032767018	0.9995114802149487054225696141	45	1
140.2592013164867	143.1216339964150	0.9995725451880801172447484123	49	1
148.8464993562716	151.7089320361999	0.9996336101612115290669272106	52	1
157.4337973960565	160.2962300759848	0.999755740107474352711284807	55	2
160.2962300759848	163.1586627559131	0.9998168050806057645334636053	56	1
163.1586627559131	166.0210954358414	0.9998778700537371763556424035	57	1
177.4708261555546	180.3332588354829	0.9999389350268685881778212018	62	1
340.6294889114677	343.4919215913960	1	119	1";

        public static readonly List<JiraIssueBucket> Buckets = new List<JiraIssueBucket>();

        static SprintPointValueService()
        {
            string[] lines = SprintPointValues.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string line in lines)
            {
                JiraIssueBucket newBucket = new JiraIssueBucket();
                newBucket.Parse(line);
                Buckets.Add(newBucket);
            }
        }

        public decimal GetPointValueForHours(decimal hours)
        {
            decimal? points = Buckets.Where(b => b.EndValue >= hours)
                .OrderBy(b => b.StartValue)
                .FirstOrDefault()
                ?.Points;

            return points ?? 1M;
        }

        public decimal GetPointValueForHours(double hours)
        {
            return GetPointValueForHours((decimal)hours);
        }

        public decimal GetNormalizedTotalPointValue(decimal totalPointValue, decimal projectedHours)
        {
            return (totalPointValue / (projectedHours / 6)) * 10;
        }
    }
}
