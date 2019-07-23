package com.example.myapplication.ui.main;


import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.Nullable;
import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProviders;
import com.example.myapplication.R;
/**
 * A placeholder fragment containing a simple view.
 */
public class PlaceholderFragment extends Fragment {
    private static final String ARG_SECTION_NUMBER = "section_number";
    private static final String FILE_CONTENT = "file_content";
    private PageViewModel pageViewModel;
    View root;
    public PlaceholderFragment() {
        // Required empty public constructor
    }
    public static PlaceholderFragment newInstance(int index, String content)
    {
        PlaceholderFragment fragment = new PlaceholderFragment();
        Bundle bundle = new Bundle();
        bundle.putInt(ARG_SECTION_NUMBER, index);
        bundle.putString(FILE_CONTENT, content);
        fragment.setArguments(bundle);
        return fragment;
    }
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        pageViewModel = ViewModelProviders.of(this).get(PageViewModel.class);
        int index = 1;
        if (getArguments() != null) {
            index = getArguments().getInt(ARG_SECTION_NUMBER);
        }
        pageViewModel.setIndex(index);
    }
    @Override
    public View onCreateView(
            @NonNull LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        root = inflater.inflate(R.layout.fragment_placeholder, container, false);
        final TextView textView = root.findViewById(R.id.section_label);
        pageViewModel.getText().observe(this, new Observer<String>() {
            @Override
            public void onChanged(@Nullable String s) {
                textView.setText(getArguments().getString(FILE_CONTENT));
            }
        });
        return root;
    }
}