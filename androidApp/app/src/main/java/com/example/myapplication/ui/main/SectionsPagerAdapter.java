package com.example.myapplication.ui.main;

import android.content.Context;
import android.provider.ContactsContract;

import androidx.annotation.Nullable;
import androidx.annotation.StringRes;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentPagerAdapter;

import org.json.JSONArray;
import org.json.JSONException;

/**
 * A [FragmentPagerAdapter] that returns a fragment corresponding to
 * one of the sections/tabs/pages.
 */
public class SectionsPagerAdapter extends FragmentPagerAdapter {
    int mNumOfTabs = 5;
    Fragment fragment = null;

    // @StringRes
//    private static final int[] TAB_TITLES = new int[]{R.string.tab_text_1, R.string.tab_text_2, R.string.tab_text_3};
    private final Context mContext;
    //    private static final String TITLE = "TAB ";
    String [] titles = {"main.xml", "main.txt", "log.txt", "readME.txt", "hello.txt"};
    String[] content = {"you are inside main.xml",
            "you are inside main.txt",
            "you are inside log.txt",
            "you are inside readME.txt",
            "you are inside hello.txt"};
    private JSONArray filesAndContents;
    public SectionsPagerAdapter(Context context, FragmentManager fm, JSONArray filesAndContents)
    {
        super(fm);
        mContext = context;
        this.filesAndContents = filesAndContents;
        mNumOfTabs = filesAndContents.length();
    }
    @Override
    public Fragment getItem(int position)
    {
        String data = "EmptyData";
        try
        {
            data = this.filesAndContents.getJSONObject(position).getString("Data");
        }catch (JSONException e)
        {

        }
        fragment = PlaceholderFragment.newInstance(position+1, data);
        return fragment;
    }
    //    @Nullable
//    @Override
//    public CharSequence getPageTitle(int position) {
//        return mContext.getResources().getString(TAB_TITLES[position]);
//    }
    @Nullable
    @Override
    public CharSequence getPageTitle(int position)
    {
        String title = "EmptyTitle";
        try
        {
            title = this.filesAndContents.getJSONObject(position).getString("Name");
        }catch (JSONException e)
        {

        }
        return title;
    }
    @Override
    public int getCount() {
        return mNumOfTabs;
    }
}
