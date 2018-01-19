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
        static const AkUniqueID ALPINEMUSIC = 453755285U;
        static const AkUniqueID AMBIENTALPINE = 3337376526U;
        static const AkUniqueID BEARAPPEARDISAPPEAR = 774343659U;
        static const AkUniqueID BEARSWAY = 851259619U;
        static const AkUniqueID BEARVOCAL = 1343055514U;
        static const AkUniqueID CRABMOVEMENT = 1662606220U;
        static const AkUniqueID DUCKFLY = 3180804649U;
        static const AkUniqueID DUCKVOCAL = 3182212785U;
        static const AkUniqueID FROGVOCAL = 1123537534U;
        static const AkUniqueID INTERACTFISHPICKUP = 3981837689U;
        static const AkUniqueID INTERACTFISHPICKUPLOOP = 2920868259U;
        static const AkUniqueID INTERACTPENGUINDROP = 41979954U;
        static const AkUniqueID INTERACTPENGUINPICKUP = 3091785893U;
        static const AkUniqueID INTERACTWATERSPLASH = 566451541U;
        static const AkUniqueID OCTOPUSAPPEARDISAPPEAR = 1938830774U;
        static const AkUniqueID OCTOPUSVOCAL = 3164774671U;
        static const AkUniqueID OTTERMOVEMENT = 3615308082U;
        static const AkUniqueID OTTERVOCAL = 3510304056U;
        static const AkUniqueID PENGUINQUACK = 3719585952U;
        static const AkUniqueID PENGUINSWIM = 4055301235U;
        static const AkUniqueID POLARAMBIENCE = 4107323607U;
        static const AkUniqueID POLARMUSIC = 3060268180U;
        static const AkUniqueID SALMONSCARE = 3536762137U;
        static const AkUniqueID SEAGULLFLY = 1191252457U;
        static const AkUniqueID SEAGULLPOOP = 4275580380U;
        static const AkUniqueID SEAGULLVOCAL = 513789297U;
        static const AkUniqueID WATERSPLASH = 777507535U;
        static const AkUniqueID WHALEBREACH = 2933929543U;
        static const AkUniqueID WHALESPRAY = 893632031U;
        static const AkUniqueID WHALESWIM = 1540104444U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace OCTOPUSMINIGAME
        {
            static const AkUniqueID GROUP = 610585235U;

            namespace STATE
            {
                static const AkUniqueID OFF = 930712164U;
                static const AkUniqueID ON = 1651971902U;
            } // namespace STATE
        } // namespace OCTOPUSMINIGAME

    } // namespace STATES

    namespace SWITCHES
    {
        namespace ALPINEMUSICSWITCH
        {
            static const AkUniqueID GROUP = 4010995541U;

            namespace SWITCH
            {
            } // namespace SWITCH
        } // namespace ALPINEMUSICSWITCH

        namespace BEARAPPEARDISAPPEAR
        {
            static const AkUniqueID GROUP = 774343659U;

            namespace SWITCH
            {
                static const AkUniqueID APPEAR = 3031620102U;
                static const AkUniqueID DISAPPEAR = 2455326200U;
            } // namespace SWITCH
        } // namespace BEARAPPEARDISAPPEAR

        namespace OCTOPUSAPPEARDISAPPEAR
        {
            static const AkUniqueID GROUP = 1938830774U;

            namespace SWITCH
            {
                static const AkUniqueID APPEAR = 3031620102U;
                static const AkUniqueID DISAPPEAR = 2455326200U;
            } // namespace SWITCH
        } // namespace OCTOPUSAPPEARDISAPPEAR

        namespace PENGUINQUACK
        {
            static const AkUniqueID GROUP = 3719585952U;

            namespace SWITCH
            {
                static const AkUniqueID CALL = 3753286133U;
                static const AkUniqueID CLIMB = 1819394456U;
                static const AkUniqueID EAT = 781390793U;
                static const AkUniqueID IDLE = 1874288895U;
                static const AkUniqueID TAPPED = 195108171U;
                static const AkUniqueID THROWN = 2260218479U;
            } // namespace SWITCH
        } // namespace PENGUINQUACK

        namespace PENGUINSWIM
        {
            static const AkUniqueID GROUP = 4055301235U;

            namespace SWITCH
            {
                static const AkUniqueID FAST = 2965380179U;
                static const AkUniqueID FASTUNDERWATER = 2059843340U;
                static const AkUniqueID SLOW = 787604482U;
                static const AkUniqueID SLOWUNDERWATER = 3949372325U;
            } // namespace SWITCH
        } // namespace PENGUINSWIM

        namespace POLARMUSICSWITCH
        {
            static const AkUniqueID GROUP = 631772696U;

            namespace SWITCH
            {
            } // namespace SWITCH
        } // namespace POLARMUSICSWITCH

        namespace WATERSPLASH
        {
            static const AkUniqueID GROUP = 777507535U;

            namespace SWITCH
            {
                static const AkUniqueID LARGE = 4284352190U;
                static const AkUniqueID MEDIUM = 2849147824U;
                static const AkUniqueID SMALL = 1846755610U;
                static const AkUniqueID SMALLMED = 3212594608U;
                static const AkUniqueID SMALLSOFT = 1108843758U;
            } // namespace SWITCH
        } // namespace WATERSPLASH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID PENGUINSIZE = 145924928U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID ALPINE = 3781746684U;
        static const AkUniqueID POLAR = 2194252879U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MASTER_SECONDARY_BUS = 805203703U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID SFX = 393239870U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID REVERB = 348963605U;
    } // namespace AUX_BUSSES

}// namespace AK

#endif // __WWISE_IDS_H__
