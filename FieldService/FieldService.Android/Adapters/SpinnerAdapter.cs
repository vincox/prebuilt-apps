//
//  Copyright 2012  Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using FieldService.Data;

namespace FieldService.Android {
    /// <summary>
    /// Generic adapter for Spinners throughout the app
    /// </summary>
    public class SpinnerAdapter<T> : BaseAdapter, ISpinnerAdapter {
        List<string> items;
        Context context;
        int resourceId;
        public SpinnerAdapter (T[] items, Context context, int resourceId)
            : base ()
        {
            this.items = new List<string> ();
            foreach (var item in items) {
                this.items.Add (item.ToString());
            }
            this.context = context;
            this.resourceId = resourceId;
        }

        public override Java.Lang.Object GetItem (int position)
        {
            return items [position];
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override long GetItemId (int position)
        {
            return (long)position;
        }

        /// <summary>
        /// Text color for the item
        /// </summary>
        public Color TextColor
        {
            get;
            set;
        }

        /// <summary>
        /// Background color of the text view
        /// </summary>
        public Color Background
        {
            get;
            set;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
			var item = items.ElementAtOrDefault (position);
			//In order to avoid an error in inflating the spinner objects in 4.4+ make them here.
			if ((int)Build.VERSION.SdkInt >= 19) {

				if (convertView != null && convertView is TextView) {
					((TextView)convertView).Text = item;
					return convertView;
				}
				var linLay = new LinearLayout (context);
				var textView = new TextView (context);
				textView.Text = item;
				textView.TextSize = 18;
				textView.SetTextColor (TextColor);
				textView.Selected = true;
				textView.FocusChange += new EventHandler<View.FocusChangeEventArgs> ((sender, e) => {
					var hasFocus = e.HasFocus;
					if (hasFocus && textView.Visibility == ViewStates.Visible) {
						//TextViews can only be ellipsized when they have focus or are selected
						textView.Ellipsize = TextUtils.TruncateAt.Marquee;
					}
				});
				textView.Gravity = GravityFlags.CenterVertical;
				textView.SetPadding (0, 10, 0, 10);
				textView.SetBackgroundDrawable (null);
				textView.LayoutParameters = new AbsListView.LayoutParams (AbsListView.LayoutParams.WrapContent, 
					AbsListView.LayoutParams.WrapContent);
				linLay.AddView (textView);
				linLay.SetBackgroundColor (Background);
				return linLay;
			} else {
				var view = convertView;
				if (view == null) {
					LayoutInflater inflator = (LayoutInflater)context.GetSystemService (Context.LayoutInflaterService);
					view = inflator.Inflate (resourceId, null);
				} else
					return view;
				var textView = view.FindViewById<TextView> (Resource.Id.simpleSpinnerTextView);
				textView.Selected = true;
				textView.Text = item;
				textView.SetTextColor (TextColor);
				textView.SetBackgroundColor (Background);
				return view;
			}
        }

        protected override void Dispose (bool disposing)
        {
            context = null;
            base.Dispose (disposing);
        }
    }
}