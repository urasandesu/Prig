/* 
 * File: Village.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2014 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */


using System;
using System.Linq;
using System.Collections.Generic;
using UntestableLibrary;

namespace program1.MyLibrary
{
    public class Village
    {
        public Village()
        {
            var r = new Random(DateTime.Now.Second);
            var count = r.Next(100);
            for (int i = 0; i < count; i++)
                m_ricePaddies.Add(new RicePaddy(i, r));

            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    var distance = r.Next(100);
                    m_roads.Add(new FarmRoad(m_ricePaddies[i], m_ricePaddies[j], distance));
                    m_roads.Add(new FarmRoad(m_ricePaddies[j], m_ricePaddies[i], distance));
                }
            }
        }

        List<RicePaddy> m_ricePaddies = new List<RicePaddy>();
        public IEnumerable<RicePaddy> RicePaddies { get { return m_ricePaddies; } }

        List<FarmRoad> m_roads = new List<FarmRoad>();
        public IEnumerable<FarmRoad> Roads { get { return m_roads; } }

        Dictionary<RicePaddy, Dictionary<RicePaddy, Route>> m_shortestRoutesMap = new Dictionary<RicePaddy, Dictionary<RicePaddy, Route>>();

        public Route GetShortestRoute(RicePaddy start, RicePaddy end)
        {
            if (!m_shortestRoutesMap.ContainsKey(start))
                m_shortestRoutesMap[start] = CalculateShortestRoutes(start);
            return m_shortestRoutesMap[start][end];
        }

        Dictionary<RicePaddy, Route> CalculateShortestRoutes(RicePaddy start)
        {
            var shortestRoutes = new Dictionary<RicePaddy, Route>();
            var handled = new List<RicePaddy>();

            foreach (var ricePaddy in m_ricePaddies)
            {
                shortestRoutes.Add(ricePaddy, new Route(ricePaddy.Identifier));
            }

            shortestRoutes[start].TotalDistance = 0;

            while (handled.Count != m_ricePaddies.Count)
            {
                var shortestRicePaddies = shortestRoutes.OrderBy(_ => _.Value.TotalDistance).Select(_ => _.Key).ToArray();

                var processing = default(RicePaddy);
                foreach (var ricePaddy in shortestRicePaddies)
                {
                    if (!handled.Contains(ricePaddy))
                    {
                        if (shortestRoutes[ricePaddy].TotalDistance == int.MaxValue)
                            return shortestRoutes;
                        processing = ricePaddy;
                        break;
                    }
                }

                var selectedRoads = m_roads.Where(_ => _.A == processing);

                foreach (var road in selectedRoads)
                {
                    if (shortestRoutes[road.B].TotalDistance > road.Distance + shortestRoutes[road.A].TotalDistance)
                    {
                        var roads = shortestRoutes[road.A].Roads.ToList();
                        roads.Add(road);
                        shortestRoutes[road.B].Roads = roads;
                        shortestRoutes[road.B].TotalDistance = road.Distance + shortestRoutes[road.A].TotalDistance;
                    }
                }
                handled.Add(processing);
            }

            return shortestRoutes;
        }
    }
}
