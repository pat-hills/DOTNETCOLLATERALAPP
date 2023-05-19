/**
 * CRNTabs.js v1.0.0
 * http://www.bsystemslimited.com
 *
 * Author: MfJunkie
 *
 * Copyright 2014, Codrops
 * http://www.codrops.com
 *
 * Copyright 2015
 * http://www.bsystemslimited.com
 */
;( function( window ) {

	'use strict';

	function extend( a, b ) {
		for( var key in b ) {
			if( b.hasOwnProperty( key ) ) {
				a[key] = b[key];
			}
		}
		return a;
	}

	function CRNTabs( el, options ) {
		this.el = el;
		this.options = extend( {}, this.options );
  		extend( this.options, options );
  		this._init();
	}

	CRNTabs.prototype.options = {
		start : 0
	};

	CRNTabs.prototype._init = function() {
		// tabs elems
		this.tabs = [].slice.call( this.el.querySelectorAll( 'nav > ul > li' ) );
		// content items
		this.items = [].slice.call( this.el.querySelectorAll( '.content-wrap > section' ) );
		// active index
		this.active = -1;
		// show active content item
		this._show();
		// init events
		this._initEvents();
	};

	CRNTabs.prototype._initEvents = function() {
		var self = this;
		this.tabs.forEach( function( tab, idx ) {
			tab.addEventListener( 'click', function( ev ) {
				ev.preventDefault();
				self._show( idx );
			} );
		} );
	};

	CRNTabs.prototype._show = function( idx ) {
		if( this.active >= 0 ) {
			this.tabs[ this.active ].className = this.items[ this.active ].className = '';
		}
		// change active
		this.active = idx != undefined ? idx : this.options.start >= 0 && this.options.start < this.items.length ? this.options.start : 0;
		this.tabs[ this.active ].className = 'tab-active';
		this.items[ this.active ].className = 'content-active';
	};

	// add to global namespace
	window.CRNTabs = CRNTabs;

})( window );
