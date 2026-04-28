//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

#if IOS || MACCATALYST

namespace Gurux.Common.Enums
{
    /// <summary>
    /// Specifies the handshaking protocol for serial port communications.
    /// </summary>
    public enum Handshake
    {
        /// <summary>
        /// No control is used for the handshake.
        /// </summary>
        None,

        /// <summary>
        /// The XON/XOFF software control protocol is used.
        /// The XOFF control is sent to stop the transmission of data.
        /// The XON control is sent to resume the transmission.
        /// These software controls are used instead of RTS/CTS hardware controls.
        /// </summary>
        XOnXOff,

        /// <summary>
        /// RTS (Request To Send) hardware flow control is used.
        /// RTS signals that data is available for transmission.
        /// </summary>
        RequestToSend,

        /// <summary>
        /// Both RTS hardware control and XON/XOFF software controls are used.
        /// </summary>
        RequestToSendXOnXOff
    }
}
#endif