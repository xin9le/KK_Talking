class InstagramVideo {
    constructor() { }


    static enableControl(document) {
        const observer = new MutationObserver(mutations => {
            for (const x of mutations) {
                if (x.type !== 'childList')
                    continue;
                this.enablePostsVideoControl(null);
                this.enableHighlightsVideoControl(null);
            }
        });
        observer.observe(document, { childList: true, subtree: true });
    }


    static enablePostsVideoControl(parent) {
        const rawVideos = this.findElements(parent, 'video.tWeCl');
        const cloneVideos = this.findElements(parent, 'video.kk_video');
        if (rawVideos.length > 0 && cloneVideos.length === 0) {  // クローンがひとつも登録されていないときだけ
            rawVideos.prop('controls', true);
            const clone = $(rawVideos.clone(false)[0]);  // ひとつだけ
            clone.removeClass('tWeCl').addClass('kk_video');            
            clone.insertAfter(rawVideos);
            rawVideos.remove();
            this.findElements(parent, 'img._8jZFn').hide();
            this.findElements(parent, 'div.PyenC').hide();
            this.findElements(parent, 'div.fXIG0').hide();
            this.findElements(parent, 'div.JSZAJ').hide();
            console.log('Enable post video control.');
        }
    }


    static enableHighlightsVideoControl(parent) {
        const videos = this.findElements(parent, 'video.y-yJ5');
        if (videos.length > 0) {
            this.findElements(parent, 'div.yxA_V').css('pointer-events', 'auto');
            videos.prop('autoplay', false);
            videos.prop('controls', true);
            videos.removeAttr('playsinline');
            videos.removeClass('OFkrO');
            videos.siblings('div.i0EQd').hide();
            console.log('Enable highlight video control.');
        }
    }


    static findElements(parent, selector) {
        return parent === undefined || parent === null
            ? $(selector)
            : $(parent).find(selector);
    }
}


class KKSearch {
    constructor() { }


    static isAvailable(url) {
        const targetUrl = 'https://www.instagram.com/kk_talking';
        return url?.toLowerCase()?.startsWith(targetUrl) ?? false;
    }


    static enable(document) {
        const observer = new MutationObserver(mutations => {
            if (!KKSearch.isAvailable(location.href))
                return;

            for (const x of mutations) {
                if (x.type === 'childList')
                    this.tryAddSearchTab();
            }
        });
        observer.observe(document, { childList: true, subtree: true });
    }


    static tryAddSearchTab() {
        //--- タブコンテナー要素が生成されているか
        const tabContainer = $('div.fx7hk');
        if (tabContainer.length === 0)
            return;

        //--- 検索タブがあるか
        if (tabContainer.children('#kk_searchTab').length > 0)
            return;

        //--- なければ検索タブを生成
        const searchTabDomText =
            `<a id="kk_searchTab" class="_9VEo1" href="/kk_talking#search">
                <span class="qzihg">
                    <div class="coreSpriteSearchIcon"></div>
                    <span class="_08DtY">KK Search</span>
                </span>
            </a>`;
        const searchTabContentDomText =
            `<div id="kk_searchTabContent">
                <div class="kk_searchBox">
                    <form>
                        <input class="XTCLo x3qfX" type="text" placeholder="Input search keyword here.">
                        <input type="submit" class="sqdOP L3NKy y3zKF" value="Search">
                    </form>
                </div>
                <div id="kk_searchResultContainer"></div>
            </div>`;
        tabContainer.append(searchTabDomText);
        tabContainer.after(searchTabContentDomText);

        //--- タブの On / Off を制御
        const tabs = tabContainer.children('a._9VEo1');
        tabs.on('click', e => {
            const activeTabClassName = 'T-jvg';
            tabs.removeClass(activeTabClassName);  // 一旦全タブを無効化

            const searchTabContent = $('#kk_searchTabContent');
            const otherTabContent = tabContainer.siblings('div._2z6nI');
            if (e.currentTarget.id === 'kk_searchTab') {
                $(e.currentTarget).addClass(activeTabClassName);  // 検索タブを有効化
                searchTabContent.show();
                otherTabContent.hide();
                $('.kk_searchBox input[type="text"]').focus();
                return false;  // a タグを無効化
            }
            else {
                searchTabContent.hide();
                otherTabContent.show();
            }
        });

        //--- 検索フォームのイベント
        this.attachSearchBoxEvent();
    }


    static attachSearchBoxEvent() {
        $('.kk_searchBox form').on('submit', async e => {
            //--- submit 処理をキャンセル
            e.preventDefault();

            //--- 入力がなければ無視
            const textBox = $(e.target).find('input[type="text"]');
            const keyword = textBox.val().trim();
            if (keyword === null || keyword === '')
                return;

            //--- 連打防止
            const button = $('.kk_searchBox input[type="submit"]');
            button.prop("disabled", true);

            //--- 要素を削除
            const container = $('#kk_searchResultContainer');
            container.empty();
            container.append(this.createSearchingElement());

            try {
                //--- 検索
                const encodedKeyword = encodeURIComponent(keyword);
                const url = 'https://kktalking.azure-api.net/instagram/v1/search?q=' + encodedKeyword;
                const result = await $.get(url);
                container.empty();

                //--- 要素を追加
                if (0 < result.contents.length) {
                    for (const x of result.contents) {
                        const element = this.createSearchResultElement(x, result.thumbnailEndpoint);
                        container.append(element);
                    }
                }
                else {
                    container.append(this.createNoResultsElement());
                }
            }
            catch (ex) {
                console.error(ex);
                container.empty();
                container.append(this.createErrorElement());
            }
            finally {
                //--- 元に戻す
                button.prop("disabled", false);
            }
        });
    }


    static createSearchResultElement(metadata, thumbnailEndpoint) {
        //--- Topics
        let topics = '';
        if (0 < metadata.topics.length) {
            topics = '<div class="kk_topics"><h2>⚜️Topic</h2><dl>';
            for (const x of metadata.topics) {
                topics +=
                    `<dt>${x.english}</dt>
                    <dd>${x.japanese}</dd>`;
            }
            topics += '</dl></div>';
        }

        //--- Tips
        let tips = '';
        if (0 < metadata.tips.length) {
            tips = '<div class="kk_tips"><h2>🍀Tips</h2><dl>';
            for (const x of metadata.tips) {
                tips +=
                    `<dt>${x.english}</dt>
                    <dd>${x.japanese}</dd>`;
            }
            tips += '</dl></div>';
        }

        //--- 要素
        const item =
            `<div class="kk_itemBox">
                <div class="kk_imageBox">
                    <a href="https://www.instagram.com/p/${metadata.shortCode}" target="_blank">
                        <img src="${thumbnailEndpoint}/KK${metadata.number}.jpg" />
                    </a>
                </div>
                <div class="kk_infoBox">
                    <a href="https://www.instagram.com/p/${metadata.shortCode}" target="_blank">
                        <h1>KK ${metadata.number}</h1>
                    </a>
                    ${topics}
                    ${tips}
                </div>
            </div>`;
        return item;
    }


    static createNoResultsElement() {
        const element =
            `<div class="Igw0E rBNOH eGOV_ _4EzTm">
                <div class="Igw0E rBNOH eGOV_ _4EzTm oaeHW K7QFQ _6wM3Z sn5rQ" style="max-width: 350px;">
                    <div class="Igw0E IwRSH eGOV_ _4EzTm FBi-h kEKum">
                        <h1 class="_7UhW9 fKFbl yUEEX KV-D4 uL8Hv l4b0S">No results found.</h1>
                    </div>
                </div>
            </div>`;
        return element;
    }


    static createErrorElement() {
        const element =
            `<div class="Igw0E rBNOH eGOV_ _4EzTm">
                <div class="Igw0E rBNOH eGOV_ _4EzTm oaeHW K7QFQ _6wM3Z sn5rQ" style="max-width: 350px;">
                    <div class="Igw0E IwRSH eGOV_ _4EzTm FBi-h kEKum">
                        <h1 class="_7UhW9 fKFbl yUEEX KV-D4 uL8Hv l4b0S">Error occured.</h1>
                    </div>
                </div>
            </div>`;
        return element;
    }


    static createSearchingElement() {
        const element =
            `<div class="Igw0E rBNOH eGOV_ _4EzTm">
                <div class="Igw0E rBNOH eGOV_ _4EzTm oaeHW K7QFQ _6wM3Z sn5rQ" style="max-width: 350px;">
                    <progress></progress>
                    <div class="Igw0E IwRSH eGOV_ _4EzTm FBi-h kEKum">
                        <h1 class="_7UhW9 fKFbl yUEEX KV-D4 uL8Hv l4b0S">Now searching...</h1>
                    </div>
                </div>
            </div>`
        return element;
    }
}


(function () {
    InstagramVideo.enableControl(document);
    KKSearch.enable(document);
}());
