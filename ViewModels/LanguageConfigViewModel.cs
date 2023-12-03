using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Services;
using Ninject;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace D2MTranslator.ViewModels
{
    class LanguageConfigViewModel : ObservableObject
    {
        private bool _deDE;
        public bool deDE { get =>_deDE; set => SetPropertySentMessage(ref _deDE, value, nameof(deDE)); }
        private bool _esES;
        public bool esES { get => _esES; set => SetPropertySentMessage(ref _esES, value, nameof(esES)); }
        private bool _esMX;
        public bool esMX { get => _esMX; set => SetPropertySentMessage(ref _esMX, value, nameof(esMX)); }
        private bool _frFR;
        public bool frFR { get => _frFR; set => SetPropertySentMessage(ref _frFR, value, nameof(frFR)); }
        private bool _itIT;
        public bool itIT { get => _itIT; set => SetPropertySentMessage(ref _itIT, value, nameof(itIT)); }
        private bool _jaJP;
        public bool jaJP { get => _jaJP; set => SetPropertySentMessage(ref _jaJP, value, nameof(jaJP)); }
        private bool _koKR;
        public bool koKR { get => _koKR; set => SetPropertySentMessage(ref _koKR, value, nameof(koKR)); }
        private bool _plPL;
        public bool plPL { get => _plPL; set => SetPropertySentMessage(ref _plPL, value, nameof(plPL)); }
        private bool _ptBR;
        public bool ptBR { get => _ptBR; set => SetPropertySentMessage(ref _ptBR, value, nameof(ptBR)); }
        private bool _ruRU;
        public bool ruRU { get => _ruRU; set => SetPropertySentMessage(ref _ruRU, value, nameof(ruRU)); }
        private bool _zhCN;
        public bool zhCN { get => _zhCN; set => SetPropertySentMessage(ref _zhCN, value, nameof(zhCN)); }
        private bool _zhTW;
        public bool zhTW { get => _zhTW; set => SetPropertySentMessage(ref _zhTW, value, nameof(zhTW)); }

        private bool _skipSame;
        public bool SkipSame { get => _skipSame; set => SetPropertySentMessage(ref _skipSame, value, nameof(SkipSame)); }

        private bool SetPropertySentMessage(ref bool field, bool value, string propertyName)
        {
            if (SetProperty(ref field, value, propertyName))
            {
                WeakReferenceMessenger.Default.Send(new LanguageConfigChangedMessage(propertyName, value));
                return true;
            }
            return false;
        }

        internal void OnClose(object sender, CancelEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new LanguageConfigClosingMessage());
        }

        private ConfigurationService configurationService;
        public LanguageConfigViewModel()
        {
            //check designer mode
            if (App.Kernel != null)
            {
                configurationService = App.Kernel.Get<ConfigurationService>();
            }
            else
            {
                configurationService = new ConfigurationService();
            }
            //WeakReferenceMessenger.Default.Register<LanguageConfigLoadedMessage>(this, (r, m) =>
            //{
            //    var visibility = configurationService.LanguageVisibility;
            //    initializeVisibilities(visibility);
            //});
            initializeVisibilities(configurationService.LanguageVisibility);
        }

        private void initializeVisibilities(Dictionary<string, bool> visibility)
        {
            if (visibility.ContainsKey("deDE"))
                deDE = visibility["deDE"];
            if (visibility.ContainsKey("esES"))
                esES = visibility["esES"];
            if (visibility.ContainsKey("esMX"))
                esMX = visibility["esMX"];
            if (visibility.ContainsKey("frFR"))
                frFR = visibility["frFR"];
            if (visibility.ContainsKey("itIT"))
                itIT = visibility["itIT"];
            if (visibility.ContainsKey("jaJP"))
                jaJP = visibility["jaJP"];
            if (visibility.ContainsKey("koKR"))
                koKR = visibility["koKR"];
            if (visibility.ContainsKey("plPL"))
                plPL = visibility["plPL"];
            if (visibility.ContainsKey("ptBR"))
                ptBR = visibility["ptBR"];
            if (visibility.ContainsKey("ruRU"))
                ruRU = visibility["ruRU"];
            if (visibility.ContainsKey("zhCN"))
                zhCN = visibility["zhCN"];
            if (visibility.ContainsKey("zhTW"))
                zhTW = visibility["zhTW"];
            SkipSame = configurationService.isHidingSameTranslation;
        }


    }
}
