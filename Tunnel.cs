/**
 * Copyright (c) 2009, Joeri Bekker
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace JoeriBekker.PuttyTunnelManager
{
    public enum TunnelType
    {
        LOCAL,
        REMOTE,
        DYNAMIC
    }

    class Tunnel
    {
        private Session session;

        private int sourcePort;
        private string destination;
        private int destinationPort;
        private TunnelType type;

        public Tunnel(Session session, int sourcePort, string destination, int destinationPort, TunnelType type)
        {
            this.session = session;

            this.sourcePort = sourcePort;
            this.destination = destination;
            this.destinationPort = destinationPort;
            this.type = type;
        }

        public int SourcePort
        {
            get { return this.sourcePort; }
        }

        public string Destination
        {
            get { return this.destination; }
        }

        public int DestinationPort
        {
            get { return this.destinationPort; }
        }

        public TunnelType Type
        {
            get { return this.type; }
        }

        public Session Session
        {
            get { return this.session; }
        }

        public string Serialize()
        {
            string source = this.type.ToString().Substring(0, 1) + this.sourcePort;
            if (this.type == TunnelType.DYNAMIC)
                return source + ",";
            else
                return source + "=" + this.destination + ":" + this.destinationPort + ",";
        }

        public static Tunnel Load(Session session, string data)
        {
			int sourcePort = 0;
			string destination = "";
			int destinationPort = 0;
			TunnelType type = TunnelType.LOCAL;

			var match = Regex.Match(data, "(?<ipprotocol>[46])?(?<tunnelType>[LRD])(?<sourcePort>[0-9]*)=(?<destination>[^:]*):(?<destinationPort>[0-9]*)|(?<ipprotocol>[46])?(?<tunnelType>[LRD])(?<sourcePort>[0-9]*)");
						
			if (match.Success)
			{
				if (match.Groups["ipprotocol"].Success)
				{
					// implement ip protocol handling
				}

				if(match.Groups["tunnelType"].Success)
				{
					switch (match.Groups["tunnelType"].Value)
					{
						default:
						case "L": type = TunnelType.LOCAL; break;
						case "R": type = TunnelType.REMOTE; break;
						case "D": type = TunnelType.DYNAMIC; break;
					}
				}

				if(match.Groups["sourcePort"].Success)
				{
					sourcePort = Int32.Parse(match.Groups["sourcePort"].Value);
				}

				if (match.Groups["destination"].Success)
				{
					destination = match.Groups["destination"].Value;
				}
				
				if(match.Groups["destinationPort"].Success)
				{
					destinationPort = Int32.Parse(match.Groups["destinationPort"].Value);
				}
			}

            return new Tunnel(session, sourcePort, destination, destinationPort, type);
        }

        public override bool Equals(object obj)
        {
            Tunnel tunnel = obj as Tunnel;
            if (tunnel == null)
            {
                return base.Equals(obj);
            }
            else
            {
                return (this.session == tunnel.Session &&
                    this.destination == tunnel.Destination &&
                    this.destinationPort == tunnel.DestinationPort &&
                    this.sourcePort == tunnel.SourcePort &&
                    this.type == tunnel.Type
                );
            }
        }

        public override int GetHashCode()
        {
            // TODO: This should be calculated based on the equals stuff that is now done in the equals method.
            return base.GetHashCode();
        }
    }
}
