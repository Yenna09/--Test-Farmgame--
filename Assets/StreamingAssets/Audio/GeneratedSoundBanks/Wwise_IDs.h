/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID PLAY_DAYNIGHTSTART = 2577162886U;
        static const AkUniqueID PLAY_FOOTSTEPS = 3854155799U;
        static const AkUniqueID PLAY_MAINMENU = 3738780720U;
        static const AkUniqueID PLAY_MUSICAYAMBIENTE = 2596494084U;
        static const AkUniqueID PLAY_PLOWING = 2770939616U;
        static const AkUniqueID PLAY_WATERINGCAN = 346398545U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GROUND
        {
            static const AkUniqueID GROUP = 2528658256U;

            namespace STATE
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID WOOD = 2058049674U;
            } // namespace STATE
        } // namespace GROUND

        namespace TIME
        {
            static const AkUniqueID GROUP = 2654366170U;

            namespace STATE
            {
                static const AkUniqueID DAY = 311764537U;
                static const AkUniqueID NIGHT = 1011622525U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace TIME

    } // namespace STATES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID MASTERVOLUME = 2918011349U;
        static const AkUniqueID MUSICVOLUME = 2346531308U;
        static const AkUniqueID SFXVOLUME = 988953028U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN_SOUNDBANK = 2228651116U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID ENVIRONMENTAL = 1973600711U;
        static const AkUniqueID MAIN_AUDIO_BUS = 2246998526U;
        static const AkUniqueID MUSIC = 3991942870U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
